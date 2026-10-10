# Chima — Security

## What we protect

| Layer | Control | Where |
|-------|---------|-------|
| Transport | HSTS (prod), HTTPS redirect (prod) | `Program.cs`, `next.config.ts` |
| Headers | CSP, X-Frame-Options, X-Content-Type-Options, Referrer-Policy, Permissions-Policy | `SecurityHeadersMiddleware`, Next config |
| Fingerprinting | `Server`, `X-Powered-By`, `X-AspNet-*` stripped | `SecurityHeadersMiddleware`, Kestrel |
| Auth (user) | JWT access 15min, refresh 30d, rotation + reuse detection | `AuthService`, `JwtTokenService` |
| Auth (admin) | Role claim from `admin_users`, `[AdminOnly]` policy | `JwtTokenService`, `AdminController` |
| Passwords | BCrypt workFactor 12, no plaintext, no logs | `AuthService` |
| Login brute force | 5 failures → 15min lockout | `AuthService` |
| Rate limits | Auth 10/min per IP, uploads 20/5min per user, admin 300/min | `RateLimitExtensions` |
| File uploads | Size cap 100MB, extension whitelist, quarantine (background processing), no direct execute | `FileService`, `FilesController` |
| SQL injection | EF Core parameterized queries only, no raw string concat | `AppDbContext` |
| XSS (API) | API never returns HTML; CSP `default-src 'none'` | `SecurityHeadersMiddleware` |
| XSS (web) | CSP with nonce-less but tight policy; React escapes by default | `next.config.ts` |
| CSRF | Not applicable — API is token-based, no cookies for auth | — |
| Secrets | Never in repo, env vars only, `.env` gitignored | `.gitignore`, `.env.example` |
| Startup validation | Refuses to boot with bad config | `StartupValidation` |
| Audit trail | Every admin mutation logged | `AuditLogService` |

## What we do NOT protect (yet)

| Gap | Impact | Mitigation roadmap |
|-----|--------|-------------------|
| Certificate pinning on mobile | MITM on compromised CA | Add when leaving Expo Go (Phase 15) |
| Webhook signature verification (Stripe/Play) | Spoofed webhooks | Add in Phase 9b when payments live |
| WAF / DDoS | Volumetric attacks | Cloudflare in front (free tier) |
| Email verification enforcement | Unverified accounts can use product | Optional feature flag |
| 2FA | Account takeover | Future feature |
| Encrypted-at-rest for message content | DB compromise → content readable | Future: envelope encryption per user |

## Pre-launch checklist

- [ ] Set a strong `Jwt:Secret` (64+ random chars) in production env
- [ ] Set a real `ConnectionStrings:Default` (managed Postgres, TLS on)
- [ ] Set `Cors:AllowedOrigins` to real domains (no localhost)
- [ ] Enable HTTPS termination at the reverse proxy (Railway/Render/Vercel do this)
- [ ] Rotate any dev-only secrets before public launch
- [ ] Verify `/health/ready` returns 200 from the public URL
- [ ] Configure Cloudflare (or equivalent) in front of production
- [ ] Enable Postgres backups (daily, 7-day retention minimum)
- [ ] Review `audit_logs` weekly for suspicious actions
- [ ] Set up alerting on 5xx rate, auth failures, quota hits

## Reporting a vulnerability

Email security@chima.app with:
- Description
- Steps to reproduce
- Impact assessment

We aim to acknowledge within 48 hours.

## Compliance notes

This document is a technical reference. It is NOT a legal compliance certification.
Before making compliance claims (GDPR, SOC2, HIPAA, PCI), consult a lawyer and
perform a formal audit. The product must not claim compliance it has not verified.