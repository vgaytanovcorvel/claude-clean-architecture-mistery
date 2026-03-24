# Prompt History — `todo-angular-v2` Branch

Essential prompts used with Claude Code to build the Angular 19 Todo SPA on this branch, in chronological order. Each prompt corresponds to one or more commits.

---

## 1. Scaffold the Angular SPA project

> Scaffold an Angular 19 SPA client project inside src/misteryapp.web.client.angular. Set it up as an .esproj that builds to Web.Server/wwwroot. Use standalone components, signal-based state, SCSS with ITCSS structure, and the layer-first folder layout from the rules (domain, repositories, services, state, components, pages, core). Update rules to latest version and generate a CLAUDE.md for the new project.

**Commit:** `e36f655 feat: scaffold Angular 19 SPA client and update rules to v3.2.6`

---

## 2. Implement the Todo app with glassmorphism UI

> Implement a multi-user todo app with a glassmorphism Angular UI. Features: mock auth with multiple users (localStorage-backed), full CRUD todo management, filtering by all/active/completed, and a frosted-glass card design. Follow the clean architecture layers: domain models (User, TodoItem, ApiResponse), repository services (*-api.service.ts) with localStorage mock backend, business logic services (AuthService, TodoService), signal-based state services, presentational components (todo-item, todo-input, todo-filter, todo-list-view, app-layout), and smart page components (login, home, todos) with route guards.

**Commit:** `828e044 feat: implement multi-user todo app with glassmorphism Angular UI`

---

## 3. Fix build and dev tooling issues

> Fix the .gitignore for node_modules, dist, wwwroot, and launch settings. Correct the Angular proxy target port to match the API launch settings. Track package-lock.json for reproducible builds. Scope the wwwroot gitignore to only the SPA build output path.

**Commits:**
- `f172b28 chore: update .gitignore for node_modules, dist, wwwroot, and launch settings`
- `e050759 fix: correct Angular proxy target port to match API launch settings`
- `8eb01cd chore: track package-lock.json for reproducible builds`
- `59b64d1 chore: scope wwwroot gitignore to SPA build output path`

---

## 4. Add comprehensive unit tests

> Add comprehensive unit tests for the entire Angular stack. Cover all services (AuthService, TodoService), repositories (UserApiService, TodoApiService), state services (AuthStateService, TodoStateService), all presentational components (todo-item, todo-input, todo-filter, todo-list-view, app-layout), the auth guard, and all page components (login, home, todos). Use Jasmine + Karma with AAA pattern and describe/it naming convention. Target 119+ specs.

**Commits:**
- `ffd5402 test: add comprehensive unit tests for entire stack (119 tests)`
- `f8ddc37 test: expand page tests and add remaining component/guard specs`

---

## Notes

- **Testing quirk:** Use `--browsers=ChromeHeadlessNoSandbox` when running `ng test` — the default `ChromeHeadless` times out and needs the NoSandbox flag.
- **Jasmine:** `toHaveBeenCalledOnce()` doesn't exist — use `toHaveBeenCalledTimes(1)`. `toHaveBeenCalledOnceWith(args)` does exist.
- **resource() API:** Use `params` (not `request`) property — returning `undefined` from params skips the load. Call `.reload()` after mutations.
- **SCSS:** `@use` must come before any CSS output — no `@layer {}` wrappers in partials; use source order for ITCSS cascade.
- All prompts were executed with Claude Code using the clean architecture rules in `rules/` and per-module `CLAUDE.md` files for context.
