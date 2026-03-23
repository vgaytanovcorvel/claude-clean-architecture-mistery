# misteryapp.web.client.react

React 19 SPA frontend — clean architecture layers with Vite, React Query, Zustand, and Zod. Communicates with the backend exclusively via HTTP through the API client.

## Rules

@../../rules/common/coding-style.md
@../../rules/common/patterns.md
@../../rules/common/security.md
@../../rules/typescript/coding-style.md
@../../rules/typescript/css.md
@../../rules/typescript/frontend-arch.md
@../../rules/typescript/react.md
@../../rules/typescript/patterns.md
@../../rules/typescript/security.md
@../../rules/typescript/testing.md

## Module Purpose

React SPA following a layer-first clean architecture: Domain (pure types/interfaces) <- Repository (HTTP access) <- Service (business logic) <- State (React Query hooks, Zustand stores) <- Presentation (components, pages). Builds to `MisteryApp.Web.Server/wwwroot` for production serving.

## Key Contents

- `src/domain/` — Models, interfaces, errors, Result pattern (zero framework imports)
- `src/repositories/` — HTTP repository implementations using ApiClient
- `src/services/` — Business logic services (plain classes, no React imports)
- `src/state/` — React Query hooks (server state) and Zustand stores (UI state)
- `src/components/` — Presentational components (all data via props)
- `src/pages/` — Smart/container components (hook composition, minimal JSX)
- `src/core/` — Composition root (providers.tsx), ApiClient, app-wide config

## Dependency Constraints

**Allowed**: No .NET project dependencies — communicates via HTTP at runtime
**Forbidden**: Direct imports across feature boundaries in presentation layer; `fetch`/`axios` outside repositories; business logic in hooks or components
