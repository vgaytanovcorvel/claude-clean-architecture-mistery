# MisteryApp.Web.React

React 19 SPA frontend for MisteryApp, built with Vite and TypeScript.

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

This is the React SPA frontend for the MisteryApp solution. It communicates with
`MisteryApp.Web.Api` exclusively via HTTP at runtime — it has no compile-time
dependency on any .NET assembly.

## Key Contents

- `src/main.tsx` — entry point, React 19 root mount, BrowserRouter
- `src/App.tsx` — route definitions (React Router v7)
- `src/features/` — feature-sliced folders (auth, todos, profile)
- `src/hooks/` — shared custom hooks
- `src/services/` — mock/API service layer
- `src/types/` — shared TypeScript interfaces

## Dependency Constraints

**Allowed**: React 19, React Router v7, Vite, TypeScript. No external UI component
libraries — all styles hand-crafted.

**Forbidden**: No direct imports from any `MisteryApp.*` C# assembly.
All backend communication goes through `src/services/` via HTTP fetch.

## Dev Commands

```bash
npm install
npm run dev       # starts Vite dev server on http://localhost:5173
npm run build     # TypeScript check + production build
npm run preview   # preview production build locally
```
