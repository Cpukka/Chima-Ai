
### 5. Root `package.json`

```json
{
  "name": "chima",
  "version": "0.1.0",
  "private": true,
  "description": "Chima — Commercial AI Assistant Platform (Monorepo)",
  "workspaces": [
    "apps/web",
    "apps/admin",
    "apps/mobile",
    "packages/shared-types",
    "packages/shared-validation",
    "packages/shared-config"
  ],
  "scripts": {
    "dev:web": "npm --workspace apps/web run dev",
    "dev:admin": "npm --workspace apps/admin run dev",
    "dev:mobile": "npm --workspace apps/mobile run start",
    "build:web": "npm --workspace apps/web run build",
    "build:admin": "npm --workspace apps/admin run build",
    "build:types": "npm --workspace packages/shared-types run build",
    "build:validation": "npm --workspace packages/shared-validation run build",
    "build:all": "npm run build:types && npm run build:validation && npm run build:web && npm run build:admin",
    "lint": "npm run lint --workspaces --if-present",
    "format": "prettier --write \"**/*.{ts,tsx,js,jsx,json,md,css}\"",
    "docker:up": "docker compose up -d",
    "docker:down": "docker compose down",
    "docker:logs": "docker compose logs -f"
  },
  "engines": {
    "node": ">=20.0.0",
    "npm": ">=10.0.0"
  },
  "devDependencies": {
    "@types/node": "^22.7.0",
    "eslint": "^9.12.0",
    "prettier": "^3.3.3",
    "rimraf": "^6.0.1",
    "typescript": "^5.6.2"
  }
}
\`\`\`

### 6. `docker-compose.yml`

```yaml
services:
  postgres:
    image: pgvector/pgvector:pg16
    container_name: chima-postgres
    restart: unless-stopped
    environment:
      POSTGRES_USER: chima
      POSTGRES_PASSWORD: chima_dev_password
      POSTGRES_DB: chima_dev
    ports:
      - "5432:5432"
    volumes:
      - chima_pgdata:/var/lib/postgresql/data
      - ./scripts/init-db.sql:/docker-entrypoint-initdb.d/init-db.sql:ro
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U chima -d chima_dev"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: chima-redis
    restart: unless-stopped
    ports:
      - "6379:6379"
    volumes:
      - chima_redisdata:/data
    command: ["redis-server", "--appendonly", "yes"]
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

volumes:
  chima_pgdata:
    name: chima_pgdata
  chima_redisdata:
    name: chima_redisdata
\`\`\`

### 7. `scripts/init-db.sql`

```sql
-- Enable required extensions for Chima
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "vector";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Create schema for app
-- (EF Core migrations will create tables under public schema)
\`\`\`

### 8. `.env.example`

```dotenv
# =====================================================
# CHIMA — ENVIRONMENT VARIABLES (EXAMPLE)
# Copy to `.env` and fill in real values.
# NEVER commit the real `.env` file.
# =====================================================

# ---------- App ----------
APP_NAME=Chima
APP_ENV=Development
APP_BASE_URL=http://localhost:3000
API_BASE_URL=http://localhost:5080

# ---------- Database ----------
DATABASE_CONNECTION_STRING=Host=localhost;Port=5432;Database=chima_dev;Username=chima;Password=chima_dev_password

# ---------- Redis ----------
REDIS_CONNECTION_STRING=localhost:6379

# ---------- JWT ----------
JWT_SECRET=CHANGE_ME_TO_A_LONG_RANDOM_STRING_AT_LEAST_64_CHARS
JWT_ISSUER=Chima
JWT_AUDIENCE=ChimaClients
JWT_ACCESS_TOKEN_MINUTES=15
JWT_REFRESH_TOKEN_DAYS=30

# ---------- AI Provider ----------
AI_PROVIDER=openai
AI_API_KEY=sk-REPLACE_ME
AI_BASE_URL=https://api.openai.com/v1
AI_DEFAULT_MODEL=gpt-4o-mini
AI_EMBEDDING_MODEL=text-embedding-3-small

# ---------- Google OAuth ----------
GOOGLE_CLIENT_ID=
GOOGLE_CLIENT_SECRET=

# ---------- Google Play Billing ----------
GOOGLE_PLAY_PACKAGE_NAME=com.chima.aiassistant
GOOGLE_PLAY_SERVICE_ACCOUNT_JSON=

# ---------- Stripe (web payments) ----------
STRIPE_SECRET_KEY=
STRIPE_WEBHOOK_SECRET=
STRIPE_PUBLISHABLE_KEY=

# ---------- Paystack (web payments, alternative) ----------
PAYSTACK_SECRET_KEY=
PAYSTACK_PUBLIC_KEY=

# ---------- Firebase (push notifications) ----------
FIREBASE_PROJECT_ID=
FIREBASE_SERVICE_ACCOUNT_JSON=

# ---------- Storage (S3-compatible) ----------
STORAGE_PROVIDER=local
STORAGE_CONNECTION_STRING=
STORAGE_BUCKET=chima-uploads
STORAGE_PUBLIC_BASE_URL=

# ---------- Email ----------
EMAIL_PROVIDER=console
EMAIL_FROM=no-reply@chima.local
EMAIL_SMTP_HOST=
EMAIL_SMTP_PORT=587
EMAIL_SMTP_USER=
EMAIL_SMTP_PASSWORD=

# ---------- Security ----------
CORS_ALLOWED_ORIGINS=http://localhost:3000,http://localhost:3001
RATE_LIMIT_REQUESTS_PER_MINUTE=60

# ---------- Logging ----------
LOG_LEVEL=Information

# ---------- Admin bootstrap (used once on first run) ----------
ADMIN_BOOTSTRAP_EMAIL=admin@chima.local
ADMIN_BOOTSTRAP_PASSWORD=ChangeMe!123
\`\`\`

### 9. `docs/architecture.md`

```markdown
# Chima — Architecture

## Overview

Chima is a multi-client AI assistant platform with a single backend API. Clients (mobile, web, admin) never talk to AI providers or payment providers directly — the backend is the source of truth.

## Layers

### Client Layer
- **Mobile**: React Native + Expo (Android first)
- **Web**: Next.js App Router
- **Admin**: Next.js App Router (separate deployment)

### API Layer (ASP.NET Core)
Clean Architecture with 4 layers:

\`\`\`
API → Application → Domain
        ↑
   Infrastructure
\`\`\`

- **Domain**: Entities, Value Objects, Enums, Domain Exceptions. No external dependencies.
- **Application**: Use cases, DTOs, Validators, Service Interfaces. Depends on Domain.
- **Infrastructure**: EF Core, AI providers, payment providers, storage, email, push. Implements Application interfaces.
- **API**: Controllers, Middleware, Filters, DI wiring. Thin — delegates to Application.

## AI Provider Abstraction

\`\`\`csharp
public interface IAIProvider
{
    string Name { get; }
    Task<AIResponse> GenerateResponseAsync(AIRequest request, CancellationToken ct);
    IAsyncEnumerable<AIChunk> StreamResponseAsync(AIRequest request, CancellationToken ct);
    Task<string> GenerateTitleAsync(string firstUserMessage, CancellationToken ct);
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct);
    Task<ModerationResult> ModerateContentAsync(string text, CancellationToken ct);
    IReadOnlyList<AIModelDescriptor> GetSupportedModels();
}
\`\`\`

Provider selected via `AI_PROVIDER` env var. Adding a new provider = new class implementing `IAIProvider`, no other changes.

## Payment Abstraction

\`\`\`csharp
public interface IPaymentProvider
{
    string Name { get; }
    Task<PaymentVerificationResult> VerifyPurchaseAsync(PurchaseVerificationRequest request, CancellationToken ct);
    Task<SubscriptionInfo> GetSubscriptionAsync(string externalId, CancellationToken ct);
}
\`\`\`

Implementations: `GooglePlayPaymentProvider`, `StripePaymentProvider`, `PaystackPaymentProvider`.

## Storage Abstraction

\`\`\`csharp
public interface IFileStorageProvider
{
    Task<StoredFile> UploadAsync(Stream stream, string key, string contentType, CancellationToken ct);
    Task<Stream> DownloadAsync(string key, CancellationToken ct);
    Task DeleteAsync(string key, CancellationToken ct);
    Task<string> GetUrlAsync(string key, TimeSpan expiry, CancellationToken ct);
}
\`\`\`

## Data Layer

- PostgreSQL 16 (primary)
- pgvector (embeddings)
- Redis (cache, rate limit counters)

## Streaming

Server-Sent Events (SSE) over HTTPS for AI response streaming. The API proxies provider streams — clients only ever see Chima's SSE endpoint.

## Security Boundaries

- All secrets server-side only.
- JWT access tokens: 15 min. Refresh tokens: 30 days, rotated.
- Rate limits enforced at API layer.
- Usage limits enforced at API layer.
- Subscription entitlements resolved server-side, never trusted from client.
\`\`\`

### 10. `docs/database.md`

```markdown
# Chima — Database Plan

PostgreSQL 16 with pgvector.

## Core Tables (to be created in Phase 2)

### Identity & Access
- `users`
- `user_profiles`
- `refresh_tokens`
- `admin_users`
- `audit_logs`

### Conversations
- `conversations`
- `messages`
- `message_attachments`
- `message_branches` (for edited messages / regenerations)

### AI
- `ai_providers`
- `ai_models`
- `usage_records`
- `usage_limits`

### Monetization
- `subscription_plans`
- `user_subscriptions`
- `payments`
- `credit_transactions`

### Files & RAG
- `uploaded_files`
- `file_chunks` (with `embedding vector(1536)`)

### System
- `notifications`
- `device_tokens`
- `reports`
- `feedback`
- `system_settings`
- `feature_flags`

## Conventions

- Primary keys: `uuid` (default `gen_random_uuid()`)
- Timestamps: `created_at`, `updated_at` (timestamptz)
- Soft delete: `deleted_at` (nullable timestamptz) where appropriate
- FKs: `ON DELETE RESTRICT` by default; `CASCADE` only for child data that must not outlive parent (e.g., `message_attachments` → `messages`)
- Financial records (`payments`, `credit_transactions`) never cascade-delete.

## Indexes

Every FK gets an index. Add composite indexes for:
- `(user_id, created_at DESC)` on conversations
- `(conversation_id, created_at ASC)` on messages
- `(user_id, created_at DESC)` on usage_records
- `(status, created_at)` on payments
- IVFFlat index on `file_chunks.embedding`

## Migrations

EF Core migrations live in `apps/api/src/Chima.Infrastructure/Persistence/Migrations`. Seed data (roles, plans, models, settings) applied via a seeder in `Persistence/Seed`.
\`\`\`

### 11. `docs/ai-architecture.md`

```markdown
# Chima — AI Architecture

## Goals

- Never hard-code a single AI provider.
- Swap providers via configuration.
- Track token usage & cost per request.
- Support streaming, vision, embeddings, moderation.

## Provider Abstraction

\`IAIProvider\` (see architecture.md). Two initial implementations:

1. `OpenAIProvider` — official OpenAI API.
2. `CompatibleAIProvider` — any OpenAI-compatible endpoint (Azure, OpenRouter, Groq, local llama.cpp, vLLM, etc.).

## Model Registry

Models are stored in the database (`ai_models`), not hard-coded. Each model has:
- name, provider, model identifier
- input price per 1M tokens
- output price per 1M tokens
- context window
- capabilities (text, vision, files, reasoning, embeddings)
- is_active, is_default

Admins manage models via the admin dashboard.

## Request Flow

\`\`\`
Client → POST /api/conversations/{id}/messages/stream
       → MessageService.SendMessageAsync
       → UsageService.CheckQuotaAsync
       → AIService.StreamAsync
       → IAIProvider.StreamResponseAsync
       → provider HTTP call (streamed)
       → SSE back to client
       → On completion: UsageService.RecordAsync
\`\`\`

## Cost Tracking

For each request, the backend records:
- input_tokens, output_tokens
- model_id
- estimated_cost (computed from model pricing)
- duration_ms

Cost is computed server-side only.

## Moderation

Before sending to provider, optionally call `ModerateContentAsync`. Policies configurable via feature flags and system settings.

## Prompt Safety

System prompts live server-side only. The client cannot request them. Prompt-injection mitigation: role separation, no raw user text in system role, optional moderation.

## RAG

Files → extract → chunk → embed → store in `file_chunks`. At query time: embed user query → top-K similarity → inject chunks as context → cite sources in response metadata.
\`\`\`

### 12. `scripts/setup.ps1`

```powershell
# Chima — Windows Setup Script
# Usage: .\scripts\setup.ps1

$ErrorActionPreference = "Stop"

Write-Host "=== Chima Setup ===" -ForegroundColor Cyan

# 1. Check prerequisites
Write-Host "`nChecking prerequisites..." -ForegroundColor Yellow

function Test-Command($cmd, $name) {
    if (Get-Command $cmd -ErrorAction SilentlyContinue) {
        Write-Host "  [OK] $name" -ForegroundColor Green
    } else {
        Write-Host "  [MISSING] $name" -ForegroundColor Red
        throw "$name is required but not installed."
    }
}

Test-Command node "Node.js"
Test-Command npm "npm"
Test-Command dotnet ".NET SDK"
Test-Command git "Git"
Test-Command docker "Docker"

# 2. Install root deps
Write-Host "`nInstalling root npm dependencies..." -ForegroundColor Yellow
npm install

# 3. Start docker services
Write-Host "`nStarting Docker services (Postgres + Redis)..." -ForegroundColor Yellow
docker compose up -d

Write-Host "`nWaiting for Postgres to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 8

# 4. Restore .NET
Write-Host "`nRestoring .NET solution..." -ForegroundColor Yellow
Push-Location apps\api
dotnet restore
Pop-Location

Write-Host "`n=== Setup complete ===" -ForegroundColor Cyan
Write-Host "Next steps:" -ForegroundColor White
Write-Host "  1. Copy .env.example to .env and fill in values"
Write-Host "  2. cd apps\api ; dotnet run --project src\Chima.Api"
Write-Host "  3. cd apps\web ; npm install ; npm run dev"
Write-Host "  4. cd apps\admin ; npm install ; npm run dev"
Write-Host "  5. cd apps\mobile ; npm install ; npx expo start"
\`\`\`

### 13. `apps/api/Directory.Build.props`

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <GenerateDocumentationFile>false</GenerateDocumentationFile>
    <InvariantGlobalization>false</InvariantGlobalization>
    <UserSecretsId>chima-api-secrets</UserSecretsId>
  </PropertyGroup>

  <PropertyGroup>
    <Authors>Chima</Authors>
    <Company>Chima</Company>
    <Product>Chima AI Assistant</Product>
    <Version>0.1.0</Version>
  </PropertyGroup>
</Project>
\`\`\`

### 14. `apps/api/.editorconfig`

```ini
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

[*.{cs,csx}]
csharp_style_namespace_declarations = file_scoped:suggestion
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_expression_bodied_methods = when_on_single_line:suggestion
dotnet_sort_system_directives_first = true
dotnet_style_qualification_for_field = false:suggestion
dotnet_style_qualification_for_property = false:suggestion
dotnet_style_qualification_for_method = false:suggestion
dotnet_style_qualification_for_event = false:suggestion
\`\`\`

### 15. `packages/shared-types/package.json`

```json
{
  "name": "@chima/shared-types",
  "version": "0.1.0",
  "private": true,
  "main": "dist/index.js",
  "types": "dist/index.d.ts",
  "scripts": {
    "build": "tsc -p tsconfig.json",
    "clean": "rimraf dist"
  },
  "devDependencies": {
    "typescript": "^5.6.2",
    "rimraf": "^6.0.1"
  }
}
\`\`\`

### 16. `packages/shared-types/tsconfig.json`

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "ESNext",
    "moduleResolution": "Bundler",
    "declaration": true,
    "outDir": "dist",
    "rootDir": "src",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true,
    "forceConsistentCasingInFileNames": true
  },
  "include": ["src/**/*"]
}
\`\`\`

### 17. `packages/shared-types/src/api-response.ts`

```typescript
export interface ApiResponse<T = unknown> {
  success: boolean;
  data: T | null;
  message: string | null;
  errors: ApiError[];
}

export interface ApiError {
  code: string;
  field?: string;
  message: string;
}

export interface PaginatedResult<T> {
  items: T[];
  nextCursor: string | null;
  hasMore: boolean;
  total?: number;
}
\`\`\`

### 18. `packages/shared-types/src/index.ts`

```typescript
export * from './api-response';
\`\`\`

### 19. `packages/shared-validation/package.json`

```json
{
  "name": "@chima/shared-validation",
  "version": "0.1.0",
  "private": true,
  "main": "dist/index.js",
  "types": "dist/index.d.ts",
  "scripts": {
    "build": "tsc -p tsconfig.json",
    "clean": "rimraf dist"
  },
  "dependencies": {
    "zod": "^3.23.8"
  },
  "devDependencies": {
    "typescript": "^5.6.2",
    "rimraf": "^6.0.1"
  }
}
\`\`\`

### 20. `packages/shared-validation/tsconfig.json`

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "module": "ESNext",
    "moduleResolution": "Bundler",
    "declaration": true,
    "outDir": "dist",
    "rootDir": "src",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true
  },
  "include": ["src/**/*"]
}
\`\`\`

### 21. `packages/shared-validation/src/index.ts`

```typescript
export {};
\`\`\`

### 22. `packages/shared-config/package.json`

```json
{
  "name": "@chima/shared-config",
  "version": "0.1.0",
  "private": true,
  "main": "index.js",
  "files": ["eslint", "typescript"]
}
\`\`\`

### 23. `apps/web/package.json` (placeholder for Phase 7)

```json
{
  "name": "@chima/web",
  "version": "0.1.0",
  "private": true,
  "scripts": {
    "dev": "next dev -p 3000",
    "build": "next build",
    "start": "next start -p 3000",
    "lint": "next lint"
  },
  "dependencies": {
    "next": "^15.0.0",
    "react": "^18.3.1",
    "react-dom": "^18.3.1"
  },
  "devDependencies": {
    "@types/node": "^22.7.0",
    "@types/react": "^18.3.10",
    "@types/react-dom": "^18.3.0",
    "typescript": "^5.6.2"
  }
}
\`\`\`

### 24. `apps/admin/package.json` (placeholder for Phase 11)

```json
{
  "name": "@chima/admin",
  "version": "0.1.0",
  "private": true,
  "scripts": {
    "dev": "next dev -p 3001",
    "build": "next build",
    "start": "next start -p 3001",
    "lint": "next lint"
  },
  "dependencies": {
    "next": "^15.0.0",
    "react": "^18.3.1",
    "react-dom": "^18.3.1"
  },
  "devDependencies": {
    "@types/node": "^22.7.0",
    "@types/react": "^18.3.10",
    "@types/react-dom": "^18.3.0",
    "typescript": "^5.6.2"
  }
}
\`\`\`

### 25. `apps/mobile/package.json` (placeholder for Phase 8)

```json
{
  "name": "@chima/mobile",
  "version": "0.1.0",
  "private": true,
  "main": "expo-router/entry",
  "scripts": {
    "start": "expo start",
    "android": "expo start --android",
    "ios": "expo start --ios",
    "web": "expo start --web",
    "lint": "expo lint"
  },
  "dependencies": {
    "expo": "^51.0.0",
    "expo-router": "^3.5.0",
    "expo-status-bar": "^1.12.1",
    "react": "^18.3.1",
    "react-native": "^0.75.0",
    "react-native-safe-area-context": "^4.10.0",
    "react-native-screens": "^3.34.0"
  },
  "devDependencies": {
    "@babel/core": "^7.24.0",
    "@types/react": "^18.3.10",
    "typescript": "^5.6.2"
  }
}
\`\`\`

---

## DATABASE

**No schema changes in Phase 1.** This phase only:
- Configures PostgreSQL + pgvector + Redis via Docker Compose.
- Enables extensions (`uuid-ossp`, `pgcrypto`, `vector`, `pg_trgm`).

Actual tables are created in **Phase 2** via EF Core migrations.

---

## ENVIRONMENT VARIABLES

All variables are listed in `.env.example`. In Phase 1, only these are needed to verify the stack:

| Variable | Purpose | Phase 1 required? |
|----------|---------|-------------------|
| `DATABASE_CONNECTION_STRING` | Postgres connection | Yes (for later phases) |
| `REDIS_CONNECTION_STRING` | Redis connection | Yes (for later phases) |
| `APP_ENV` | Environment | Optional |
| `LOG_LEVEL` | Logging | Optional |

---

## RUN

### Start infrastructure

```powershell
cd C:\Projects\chima
docker compose up -d