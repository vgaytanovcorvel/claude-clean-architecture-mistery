# MisteryApp.Web.React

React SPA — functional components, hooks-based state, and routing wired to the MisteryApp API.

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

React + TypeScript SPA built with Vite. Uses functional components with hooks for state management and React Router for client-side navigation. Communicates with `MisteryApp.Web.Api` or `MisteryApp.Web.Server` via HTTP.

## Key Contents

- `src/main.tsx` — `ReactDOM.createRoot` entry point
- `src/App.tsx` — root component with router setup

## Dependency Constraints

**Consumes**: MisteryApp.Web.Api or MisteryApp.Web.Server (HTTP only)
**Forbidden**: Direct project references to any C# assembly
