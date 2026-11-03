# Security Policy

## Supported versions

Same API is under active development. Only the `main` branch, which is what runs in production, receives security fixes.

## Reporting a vulnerability

**Please do not report security vulnerabilities through public GitHub issues.**

Use one of the following:

- GitHub [private vulnerability reporting](../../security/advisories/new) (Security tab > Report a vulnerability)
- Email: [same@sames.school](mailto:same@sames.school)

Please include:

- A description of the vulnerability and its potential impact
- Steps to reproduce (requests, payloads, affected endpoints)
- A suggested fix, if you have one

Do not include real user data or credentials in your report.

## What to expect

- Acknowledgement within **72 hours**
- An initial assessment within **7 days**
- Updates until the issue is resolved
- Credit in the release notes if you wish

## Scope

**In scope:** the API code in this repository (authentication, authorization, data exposure, injection, etc.).

**Out of scope:**

- Vulnerabilities in third-party services (Firebase, Google Cloud, GitHub)
- Social engineering and phishing
- Denial-of-service and automated high-volume testing

## Testing guidelines

- Only test with accounts you created yourself.
- Do not access, modify or delete data that belongs to other users.
- Stop and report as soon as you confirm a vulnerability.

## Disclosure

Please give us a reasonable amount of time to fix the issue before any public disclosure.