# Chima — Commercial AI Assistant Platform

> A full-stack, production-ready AI assistant shipped as four connected products from one codebase: a web app, an Android app, an admin dashboard, and a REST API.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Next.js](https://img.shields.io/badge/Next.js-15-000000?logo=next.js&logoColor=white)](https://nextjs.org/)
[![React Native](https://img.shields.io/badge/React_Native-0.86-61DAFB?logo=react&logoColor=white)](https://reactnative.dev/)
[![Expo](https://img.shields.io/badge/Expo-SDK_57-000020?logo=expo&logoColor=white)](https://expo.dev/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)

---

## What this is

Chima is a self-hostable AI assistant platform built as a real product — not a demo. It includes user accounts, streaming chat, document Q&A, subscriptions, usage limits, an operations console, and native mobile support.

Designed and built solo to demonstrate end-to-end product engineering: system design, security, cost accounting, and multi-platform delivery.

## The four products

| Product | Stack | Location |
|---------|-------|----------|
| **REST API** | ASP.NET Core 10, EF Core 9, PostgreSQL 18 + pgvector | `apps/api` |
| **Web app** | Next.js 15, React 19, TypeScript, Tailwind | `apps/web` |
| **Admin dashboard** | Next.js 15, Recharts, Tailwind | `apps/admin` |
| **Android app** | React Native 0.86, Expo SDK 57, TypeScript | `apps/mobile` |

## Feature highlights

### Assistant
- Streaming chat over **Server-Sent Events** with client-side cancellation
- Automatic conversation titles, branching for edits and regenerations
- Markdown rendering with syntax-highlighted code blocks and copy buttons
- Conversation history with search, pinning, date grouping
- File uploads (PDF, DOCX, CSV, TXT, MD, JSON) with a **RAG pipeline**: extract → chunk → embed → pgvector → semantic retrieval

### Platform
- **JWT authentication** with refresh token rotation and reuse detection
- **Admin role enforcement** via role claims resolved from the database
- **Provider-agnostic AI layer** (`IAIProvider`) — swap OpenAI for any compatible endpoint via env var
- **Cost accounting** — every request records tokens, resolves pricing from the database, computes per-request cost
- **Usage quotas** enforced server-side, per plan, per month
- **Subscription infrastructure** with plans and cancel/restore
- **Audit logging** for every admin action
- **Security headers**, startup config validation, brute-force protection

### Admin console
- Dashboard with users, revenue, AI cost, gross margin charts
- User management with suspend/unsuspend and plan changes
- Payments and subscriptions tables with CSV export
- AI provider and model CRUD with pricing configuration
- Feature flags and system settings editable at runtime
- Paginated audit log of all admin actions

## Architecture



**Clean Architecture** with the dependency rule enforced by project references.

## Tech stack

| Layer | Technology |
|-------|------------|
| **Backend** | ASP.NET Core 10, EF Core 9, FluentValidation, Serilog, BCrypt, JWT |
| **Database** | PostgreSQL 18 with pgvector |
| **AI** | Provider abstraction; OpenAI by default, any OpenAI-compatible endpoint by config |
| **Web** | Next.js 15, React 19, TypeScript, Tailwind, TanStack Query, Zustand, Recharts |
| **Mobile** | React Native, Expo SDK 57, Expo Router, SecureStore |
| **DevOps** | Docker Compose for local Postgres + Redis |

## Getting started

### Prerequisites

| Tool | Version |
|------|---------|
| Node.js | 20+ |
| .NET SDK | 10.0 |
| PostgreSQL | 18 (with `pgvector`) |
| Git | latest |

### Backend

```powershell
cd apps/api
dotnet restore
dotnet build
dotnet run --project src\Chima.Api
