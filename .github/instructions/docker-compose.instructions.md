---
description: 'Best practices for docker-compose files with Traefik labels, PostgreSQL, health checks, environment patterns, and network configuration for Portainer deployment.'
applyTo: '**/docker-compose*.yml,**/compose*.yml'
---
# Docker Compose Best Practices

## Your Mission

Guide the creation of production-ready docker-compose files for deployment to Proxmox/Portainer with Traefik reverse proxy, following best practices for service orchestration, health checks, and security.

## Core Principles

1. **Service dependencies** - Use health checks and `depends_on` conditions for proper startup order
2. **Network isolation** - Separate internal and external (Traefik) networks
3. **Environment management** - Use `.env` files with required variable validation
4. **Traefik integration** - Configure labels for routing, TLS, and load balancing

## Compose File Structure

### Production Compose Template

```yaml
# ✅ GOOD: Complete production compose file
# Usage: docker compose -f docker-compose.prod.yml up -d

services:
  postgres:
    image: postgres:16-alpine
    container_name: myapp-db
    restart: unless-stopped
    environment:
      POSTGRES_DB: ${POSTGRES_DB:-myapp}
      POSTGRES_USER: ${POSTGRES_USER:-postgres}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:?Database password required}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - internal
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-postgres}"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    image: ghcr.io/username/myapp-api:main
    container_name: myapp-api
    restart: unless-stopped
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=${POSTGRES_DB:-myapp};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - internal
      - traefik
    labels:
      - "traefik.enable=true"
      - "traefik.docker.network=traefik"
      - "traefik.http.routers.myapp-api.rule=Host(`${DOMAIN_NAME}`) && PathPrefix(`/api`)"
      - "traefik.http.routers.myapp-api.entrypoints=websecure"
      - "traefik.http.routers.myapp-api.tls=true"
      - "traefik.http.routers.myapp-api.tls.certresolver=letsencrypt"
      - "traefik.http.services.myapp-api.loadbalancer.server.port=8080"

volumes:
  postgres_data:

networks:
  internal:
    driver: bridge
  traefik:
    external: true
```

## Service Dependencies

### Proper Startup Order

```yaml
# ✅ GOOD: Use conditions with health checks
services:
  migration:
    depends_on:
      postgres:
        condition: service_healthy      # Wait for healthy status
    restart: "no"                        # One-shot container

  api:
    depends_on:
      postgres:
        condition: service_healthy
      migration:
        condition: service_completed_successfully  # Wait for exit code 0

  frontend:
    depends_on:
      - api                             # Simple dependency (just started)
```

```yaml
# ❌ BAD: No conditions, race conditions possible
services:
  api:
    depends_on:
      - postgres                        # Only waits for container start, not ready
```

### Dependency Conditions

| Condition | Use Case |
|-----------|----------|
| `service_started` | Default, container is running |
| `service_healthy` | Container health check passed |
| `service_completed_successfully` | Container exited with code 0 |

## Health Checks

### PostgreSQL

```yaml
postgres:
  healthcheck:
    test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER:-postgres}"]
    interval: 10s
    timeout: 5s
    retries: 5
    start_period: 10s
```

### .NET API

```yaml
api:
  healthcheck:
    test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
    interval: 30s
    timeout: 10s
    retries: 3
    start_period: 40s
```

### Nginx Frontend

```yaml
frontend:
  healthcheck:
    test: ["CMD", "curl", "-f", "http://localhost/health"]
    interval: 30s
    timeout: 5s
    retries: 3
```

## Traefik Labels

### API Routing (Priority 10)

```yaml
labels:
  # Enable Traefik
  - "traefik.enable=true"
  - "traefik.docker.network=traefik"
  
  # HTTPS Router for /api paths
  - "traefik.http.routers.myapp-api.rule=Host(`${DOMAIN_NAME}`) && PathPrefix(`/api`)"
  - "traefik.http.routers.myapp-api.entrypoints=websecure"
  - "traefik.http.routers.myapp-api.tls=true"
  - "traefik.http.routers.myapp-api.tls.certresolver=letsencrypt"
  - "traefik.http.routers.myapp-api.priority=10"
  
  # Service port
  - "traefik.http.services.myapp-api.loadbalancer.server.port=8080"
```

### Frontend Routing (Priority 1) with HTTP Redirect

```yaml
labels:
  # Enable Traefik
  - "traefik.enable=true"
  - "traefik.docker.network=traefik"
  
  # HTTPS Router (catch-all for domain)
  - "traefik.http.routers.myapp-frontend.rule=Host(`${DOMAIN_NAME}`)"
  - "traefik.http.routers.myapp-frontend.entrypoints=websecure"
  - "traefik.http.routers.myapp-frontend.tls=true"
  - "traefik.http.routers.myapp-frontend.tls.certresolver=letsencrypt"
  - "traefik.http.routers.myapp-frontend.priority=1"
  - "traefik.http.services.myapp-frontend.loadbalancer.server.port=80"
  
  # HTTP to HTTPS redirect
  - "traefik.http.routers.myapp-http.rule=Host(`${DOMAIN_NAME}`)"
  - "traefik.http.routers.myapp-http.entrypoints=web"
  - "traefik.http.routers.myapp-http.middlewares=myapp-https-redirect"
  - "traefik.http.middlewares.myapp-https-redirect.redirectscheme.scheme=https"
  - "traefik.http.middlewares.myapp-https-redirect.redirectscheme.permanent=true"
```

### Priority Rules

| Priority | Path Pattern | Service |
|----------|--------------|---------|
| 10+ | `/api`, `/api/*` | Backend API |
| 5 | `/admin`, `/admin/*` | Admin UI (if separate) |
| 1 | Catch-all | Frontend SPA |

## Environment Variables

### Required vs Optional Variables

```yaml
environment:
  # Required - compose fails if not set
  POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:?Database password required}
  JWT_SECRET: ${JWT_SECRET:?JWT secret required}
  
  # Optional with defaults
  POSTGRES_DB: ${POSTGRES_DB:-myapp}
  POSTGRES_USER: ${POSTGRES_USER:-postgres}
  ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-Production}
```

### .env.example Template

```bash
# Database Configuration
POSTGRES_DB=myapp
POSTGRES_USER=postgres
POSTGRES_PASSWORD=  # Required: Set a strong password

# JWT Configuration
JWT_SECRET=         # Required: Generate with: openssl rand -base64 64
JWT_ISSUER=myapp
JWT_AUDIENCE=myapp

# Domain Configuration
DOMAIN_NAME=        # Required: Your domain (e.g., app.example.com)
```

## Network Configuration

### Internal + External Pattern

```yaml
networks:
  internal:
    driver: bridge              # Isolated internal network
  traefik:
    external: true              # Pre-existing Traefik network

services:
  postgres:
    networks:
      - internal                # Database only on internal

  api:
    networks:
      - internal                # Can reach database
      - traefik                 # Exposed via Traefik

  frontend:
    networks:
      - internal                # Can reach API internally
      - traefik                 # Exposed via Traefik
```

### Create Traefik Network

```bash
# Must exist before compose up
docker network create traefik
```

## Volume Management

### Named Volumes

```yaml
# ✅ GOOD: Named volumes for data persistence
volumes:
  postgres_data:                # Managed by Docker

services:
  postgres:
    volumes:
      - postgres_data:/var/lib/postgresql/data
```

### Bind Mounts (When Needed)

```yaml
# For specific host path requirements
services:
  postgres:
    volumes:
      - /mnt/data/postgres:/var/lib/postgresql/data
```

## Migration Container Pattern

### One-Shot Migration

```yaml
services:
  migration:
    image: ghcr.io/username/myapp-migrations:main
    container_name: myapp-migration
    restart: "no"                                    # Don't restart after completion
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=${POSTGRES_DB:-myapp};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - internal
```

### API Waits for Migration

```yaml
services:
  api:
    depends_on:
      migration:
        condition: service_completed_successfully    # Wait for exit code 0
```

## Restart Policies

| Policy | Use Case |
|--------|----------|
| `unless-stopped` | Production services (api, frontend, database) |
| `always` | Critical infrastructure |
| `on-failure` | Containers that may fail temporarily |
| `"no"` | One-shot containers (migrations, init scripts) |

## Compose File Naming

| File | Purpose |
|------|---------|
| `docker-compose.yml` | Default/development |
| `docker-compose.prod.yml` | Production with Traefik |
| `docker-compose.dev.yml` | Local development overrides |
| `docker-compose.override.yml` | Auto-merged with default |

## Validation Checklist

- [ ] All services have appropriate `restart` policies
- [ ] Database has health check defined
- [ ] Services use `depends_on` with conditions
- [ ] Environment variables use `${VAR:?error}` for required values
- [ ] Internal network isolates database from public access
- [ ] Traefik labels include priority for routing order
- [ ] HTTPS router uses `tls.certresolver=letsencrypt`
- [ ] HTTP router includes redirect middleware
- [ ] Migration container uses `restart: "no"`
- [ ] Named volumes used for data persistence

## Common Issues

### Service Can't Connect to Database

```yaml
# ✅ GOOD: Use service name as host
ConnectionStrings__DefaultConnection=Host=postgres;...

# ❌ BAD: Using localhost
ConnectionStrings__DefaultConnection=Host=localhost;...
```

### Traefik Not Routing

1. Ensure container is on `traefik` network
2. Verify `traefik.docker.network=traefik` label
3. Check `traefik.enable=true` label exists
4. Confirm Traefik network is external and exists

### Migration Runs Every Time

```yaml
# ✅ Correct: One-shot with condition
migration:
  restart: "no"
  
api:
  depends_on:
    migration:
      condition: service_completed_successfully
```

### Port Conflicts

```yaml
# Only expose ports for debugging, not production
services:
  postgres:
    # ports:
    #   - "5432:5432"    # Don't expose in production
```
