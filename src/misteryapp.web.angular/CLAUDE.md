# misteryapp.web.angular

Angular 19+ SPA — standalone components, signals-based state, and routing wired to the MisteryApp API.

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

Standalone Angular 19+ application. Uses signals for reactive state, the new control flow syntax (`@if`, `@for`), and standalone components throughout. Communicates with `MisteryApp.Web.Api` or `MisteryApp.Web.Server` via HTTP.

## Key Contents

- `src/app/app.component.ts` — root standalone component
- `src/app/app.routes.ts` — application routes
- `src/main.ts` — `bootstrapApplication` entry point

## Dependency Constraints

**Consumes**: MisteryApp.Web.Api or MisteryApp.Web.Server (HTTP only)
**Forbidden**: Direct project references to any C# assembly
