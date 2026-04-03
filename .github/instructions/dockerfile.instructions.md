---
description: 'Best practices for writing Dockerfiles with multi-stage builds, layer caching, security hardening, and optimization for .NET 10 and Node.js/Vite applications.'
applyTo: '**/Dockerfile*'
---
# Dockerfile Best Practices

## Your Mission

Guide the creation of optimized, secure, and maintainable Dockerfiles following industry best practices for .NET and Node.js applications deployed to Proxmox/Portainer with Traefik.

## Core Principles

1. **Multi-stage builds** - Separate build and runtime stages to minimize image size
2. **Layer caching** - Order instructions to maximize cache hits
3. **Security first** - Run as non-root, use minimal base images, no secrets in images
4. **Reproducibility** - Pin versions, use specific tags, avoid `latest`

## .NET Dockerfile Patterns

### Multi-Stage Build Structure

```dockerfile
# ✅ GOOD: Multi-stage with layer caching
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for layer caching
COPY *.slnx ./
COPY Directory.Build.props Directory.Packages.props ./
COPY src/*/*.csproj ./
RUN for file in *.csproj; do \
    dir=$(basename "$file" .csproj); \
    mkdir -p "src/$dir"; \
    mv "$file" "src/$dir/"; \
    done

# Restore (cached unless project files change)
RUN dotnet restore

# Copy source and build
COPY src/ src/
WORKDIR /src/src/MyApi
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage - minimal image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Run as non-root user
RUN adduser --disabled-password --gecos '' appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "MyApi.dll"]
```

```dockerfile
# ❌ BAD: Single stage, no caching, runs as root
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app
WORKDIR /app
ENTRYPOINT ["dotnet", "MyApi.dll"]
```

### .NET Version Selection

| .NET Version | SDK Image | Runtime Image |
|--------------|-----------|---------------|
| .NET 10 | `mcr.microsoft.com/dotnet/sdk:10.0` | `mcr.microsoft.com/dotnet/aspnet:10.0` |
| .NET 9 LTS | `mcr.microsoft.com/dotnet/sdk:9.0` | `mcr.microsoft.com/dotnet/aspnet:9.0` |
| .NET 8 LTS | `mcr.microsoft.com/dotnet/sdk:8.0` | `mcr.microsoft.com/dotnet/aspnet:8.0` |

### EF Core Migrations Dockerfile

> [!CAUTION]
> **Build-time vs Runtime gotcha**: Docker `RUN` commands execute during image build, but `CMD`/`ENTRYPOINT` execute when the container starts. If you use `--no-build` in your EF command, you MUST explicitly build during the Docker build phase, and the **configuration must match exactly** (Release vs Debug).

```dockerfile
# ✅ GOOD: Explicit build with matching configuration
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src

# Copy and restore
COPY *.slnx ./
COPY Directory.Build.props Directory.Packages.props ./
COPY src/*/*.csproj ./
RUN for file in *.csproj; do \
    dir=$(basename "$file" .csproj); \
    mkdir -p "src/$dir"; \
    mv "$file" "src/$dir/"; \
    done
RUN dotnet restore
COPY src/ src/

# Install EF tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# ⚠️ CRITICAL: Build BEFORE using --no-build flag
# Configuration MUST match between build and ef command
WORKDIR /src/src/MyApi
RUN dotnet build -c Release

# Use CMD (not ENTRYPOINT) for one-shot containers
# --configuration Release MUST match the build configuration above
# --verbose helps diagnose path/configuration issues
CMD ["sh", "-c", "dotnet ef database update \
    --project ../MyInfrastructure/MyInfrastructure.csproj \
    --configuration Release \
    --no-build \
    --verbose"]
```

```dockerfile
# ❌ BAD: Missing build step - will fail at runtime with "deps.json not found"
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src/src/MyApi
# This FAILS because --no-build expects compiled binaries that don't exist!
ENTRYPOINT ["dotnet", "ef", "database", "update", "--no-build"]
```

```dockerfile
# ❌ BAD: Configuration mismatch - builds Release but EF defaults to Debug
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release    # Outputs to bin/Release/
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src/src/MyApi
# FAILS: EF looks in bin/Debug/ by default, but we built to bin/Release/
ENTRYPOINT ["dotnet", "ef", "database", "update", "--no-build"]
```

### Migration Container Best Practices

| Practice | Reason |
|----------|--------|
| Use `CMD` not `ENTRYPOINT` | One-shot containers; easier to override for debugging |
| Use `--configuration Release` | Must match your `dotnet build -c Release` |
| Use `--no-build` | Faster startup, uses pre-compiled binaries |
| Use `--verbose` | Helps diagnose configuration/path issues |
| Set `restart: "no"` in compose | Migrations should run once, not retry forever |

## Node.js/Vite Dockerfile Patterns

### Multi-Stage Build Structure

```dockerfile
# ✅ GOOD: Multi-stage with npm ci and minimal runtime
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files for layer caching
COPY package*.json ./
RUN npm ci --silent

# Copy source and build
COPY . .
RUN npm run build

# Runtime stage - nginx for static files
FROM nginx:alpine AS runtime

# Copy custom nginx config
COPY nginx.conf /etc/nginx/nginx.conf

# Copy built static files
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

```dockerfile
# ❌ BAD: Development dependencies in production, no nginx
FROM node:22
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build
CMD ["npx", "serve", "dist"]
```

### Node.js Version Selection

| Node Version | Image Tag | Notes |
|--------------|-----------|-------|
| Node 22 LTS | `node:22-alpine` | Recommended for new projects |
| Node 20 LTS | `node:20-alpine` | Stable, widely supported |
| Node 18 LTS | `node:18-alpine` | Legacy support |

## Layer Caching Optimization

### Order of Instructions

```dockerfile
# ✅ GOOD: Least frequently changed first
FROM node:22-alpine
WORKDIR /app

# 1. System dependencies (rarely change)
RUN apk add --no-cache curl

# 2. Package manifests (change on dependency updates)
COPY package*.json ./
RUN npm ci

# 3. Source code (changes frequently)
COPY . .
RUN npm run build
```

```dockerfile
# ❌ BAD: Copying everything first invalidates cache
FROM node:22-alpine
WORKDIR /app
COPY . .                    # Any file change invalidates all layers below
RUN npm ci
RUN npm run build
```

## Security Best Practices

### Non-Root User

```dockerfile
# ✅ GOOD: Use built-in APP_UID in ASP.NET Core 8+ images
USER $APP_UID

# ✅ GOOD: For Debian-based images without APP_UID
RUN adduser --disabled-password --gecos '' appuser
USER appuser

# ✅ GOOD: For Alpine-based images
RUN adduser -D -H -s /sbin/nologin appuser
USER appuser
```

### No Secrets in Images

```dockerfile
# ❌ BAD: Secrets baked into image
ENV DATABASE_PASSWORD=mysecretpassword
COPY .env /app/.env

# ✅ GOOD: Secrets passed at runtime
ENV DATABASE_PASSWORD=""
# Set via docker-compose environment or -e flag
```

### Minimal Base Images

```dockerfile
# ✅ GOOD: Alpine variants (smaller attack surface)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
FROM node:22-alpine
FROM nginx:alpine

# ❌ AVOID: Full images unless needed
FROM mcr.microsoft.com/dotnet/aspnet:10.0
FROM node:22
FROM nginx:latest
```

## .dockerignore Patterns

### For .NET Projects

```dockerignore
# Build artifacts
**/bin/
**/obj/
**/out/

# IDE and tools
.vs/
.vscode/
*.user
*.suo

# Git
.git/
.gitignore

# Documentation
*.md
LICENSE

# Tests (if not needed in image)
**/*Tests/
**/*Test/
```

### For Node.js Projects

```dockerignore
# Dependencies
node_modules/

# Build output (will be regenerated)
dist/
build/

# Development
.git/
.vscode/
*.md

# Environment
.env*
!.env.example

# Tests
**/*.test.*
**/*.spec.*
__tests__/
coverage/
```

## Health Checks

### .NET Health Check

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
```

### Nginx Health Check

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1
```

## Environment Variables

### .NET Container Defaults

```dockerfile
# ASP.NET Core container defaults
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_EnableDiagnostics=0

# Disable globalization invariant mode if needed
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
```

### Node.js Container Defaults

```dockerfile
# Production mode
ENV NODE_ENV=production

# Disable npm update check
ENV NPM_CONFIG_UPDATE_NOTIFIER=false
```

## Validation Checklist

- [ ] Uses multi-stage build with separate build and runtime stages
- [ ] Runtime image is minimal (alpine or similar)
- [ ] Runs as non-root user in production
- [ ] Package/project files copied before source for layer caching
- [ ] No secrets or sensitive data in image layers
- [ ] Specific version tags used (not `latest`)
- [ ] EXPOSE documents the listening port
- [ ] HEALTHCHECK defined for orchestration
- [ ] .dockerignore excludes unnecessary files
- [ ] If EF Core migrations exist: `Dockerfile.migrate` is present, uses `CMD` (not `ENTRYPOINT`), and the build configuration matches exactly (Release/Debug) between the `dotnet build` step and the `dotnet ef` command

## Troubleshooting

### Build Fails with "file not found"

```dockerfile
# Ensure WORKDIR is set before COPY
WORKDIR /src
COPY src/ src/  # Copies to /src/src/
```

### Large Image Size

1. Use multi-stage builds
2. Use alpine base images
3. Clean up package manager cache: `RUN apt-get clean && rm -rf /var/lib/apt/lists/*`
4. Combine RUN commands to reduce layers

### Permission Denied Errors

```dockerfile
# Ensure files are owned by non-root user
COPY --chown=appuser:appuser --from=build /app/publish .
```
