# misteryapp.web.client.angular

Angular 19+ SPA client — standalone components, signal-based state, clean architecture layers.

## Rules

@../../rules/common/coding-style.md
@../../rules/common/patterns.md
@../../rules/common/security.md
@../../rules/typescript/coding-style.md
@../../rules/typescript/css.md
@../../rules/typescript/frontend-arch.md
@../../rules/typescript/angular.md
@../../rules/typescript/patterns.md
@../../rules/typescript/security.md
@../../rules/typescript/testing.md

## Module Purpose

Angular SPA that communicates with the MisteryApp backend via HTTP. Follows a layer-first folder structure: domain, repositories, services, state, components, pages, core. Built output is served by MisteryApp.Web.Server via static files + SPA fallback.

## Key Contents

- `src/app/domain/` — TypeScript interfaces, models, domain errors
- `src/app/repositories/` — HTTP services (`*-api.service.ts`) wrapping `HttpClient`
- `src/app/services/` — Business logic services
- `src/app/state/` — Signal-based reactive state (`resource()`, `signal()`)
- `src/app/components/` — Presentational components + `shared/` + `layout/`
- `src/app/pages/` — Smart page components with lazy routes
- `src/app/core/` — Providers, guards, interceptors, `app.config.ts`
- `src/styles/` — ITCSS layers: abstracts, generic, base, objects, components, utilities

## Dependency Constraints

**Allowed**: No .NET project dependencies — communicates with backend exclusively via HTTP at runtime
**Build reference**: `MisteryApp.Web.Server` references this `.esproj` with `ReferenceOutputAssembly="false"`
**Output**: Builds to `../MisteryApp.Web.Server/wwwroot`
