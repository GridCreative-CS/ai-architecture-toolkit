# Update the current feature spec using the compliance report

Use as source of truth, in order:

1. final architecture
2. ADRs
3. delivery plan
4. compliance report
5. current feature spec

Rules:

- fix only what the compliance report requires
- preserve valid content
- narrow scope if needed
- add missing API, security, observability, acceptance-criteria, and test constraints where required
- do not change architecture or ADRs
- keep the feature spec decomposition-ready

At the end, include:

## Compliance Corrections Applied

- findings addressed
- changes made
- open questions
