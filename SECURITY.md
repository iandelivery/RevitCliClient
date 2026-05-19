# Security Policy

## Reporting a Vulnerability

If you discover a security vulnerability in RevitCliClient, please report it responsibly.

**Do not** report security vulnerabilities through public GitHub issues.

Instead, please report them by:

1. Opening a **private** security advisory on GitHub: [Security Advisories](../../security/advisories/new)
2. Or contacting the maintainers directly via the contact information in the repository

Please include:

- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if available)

## Security Considerations

RevitCliClient communicates with the Revit CLI Bridge server over HTTP on `localhost`. Key security aspects:

- **Localhost only** — The bridge server binds to `localhost` and is not exposed to external networks
- **No authentication** — The current implementation does not include authentication. Only run the bridge server on trusted networks
- **No TLS** — Communication is over plain HTTP. This is acceptable for localhost-only communication but should not be extended to remote connections without adding TLS
