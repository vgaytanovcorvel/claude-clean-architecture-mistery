# MisteryApp — Clean Architecture Baseline for Claude Code

A pre-scaffolded C# clean architecture solution you can clone and immediately start building with [Claude Code](https://claude.ai/code), without spending time on project setup or architecture decisions.

## What's included

- **8 C# source projects** wired with correct dependency flow (clean architecture)
- **8 C# test projects** (MSTest + Moq + FluentAssertions)
- **Angular 19 SPA** with a multi-user Todo app featuring glassmorphism UI
- **Central package management** via `Directory.Packages.props`
- **Per-module `CLAUDE.md` files** (16 total) — Claude Code reads these automatically to understand each layer's rules
- **26 architecture rule files** under `rules/` covering C#, TypeScript, Angular, CSS, persistence, testing, security, and more
- **Claude Code skills** for scaffolding and validation (`/clean-architecture:bootstrap-clean-arch`, `/simplify`)

## Architecture

### Backend (C# / .NET 9)

```
src/
├── MisteryApp.Common/          ← Shared DTOs, enums, constants
├── MisteryApp.Abstractions/    ← Domain models, interfaces, exceptions
├── MisteryApp.Implementation/  ← Service implementations, business logic
├── MisteryApp.Repository/      ← EF Core repos, DbContext, migrations
├── MisteryApp.Web.Core/        ← Controllers, filters, middleware
├── MisteryApp.Web.Api/         ← API host (Swagger, global exception handler)
├── MisteryApp.Web.Server/      ← SPA host (serves Angular + API together)
└── MisteryApp.Cli/             ← CLI host (System.CommandLine 2.0.5)

tests/
└── MisteryApp.*.Tests/         ← One test project per src project
```

Dependency flow: `Common <- Abstractions <- Implementation/Repository <- Web.Core <- Web.Api/Web.Server/Cli`

### Frontend (Angular 19 SPA)

```
src/misteryapp.web.client.angular/
├── domain/          ← TypeScript interfaces, models (User, TodoItem, ApiResponse)
├── repositories/    ← HTTP services (*-api.service.ts) — mock backend via localStorage
├── services/        ← Business logic (AuthService, TodoService)
├── state/           ← Signal-based reactive state (resource(), signal())
├── components/      ← Presentational components (todo-item, todo-input, todo-filter, layout)
├── pages/           ← Smart page components (login, home, todos)
└── core/            ← Guards, app config, routes
```

**Angular features:**
- Angular 19 standalone components with signal-based inputs/outputs
- `@if` / `@for` / `@switch` control flow syntax (no structural directives)
- `resource()` for async data loading, `signal()` / `computed()` for state
- `ChangeDetectionStrategy.OnPush` on all components
- SCSS with ITCSS layer structure and design tokens
- Glassmorphism UI design with frosted-glass card effects
- Multi-user auth (mock, localStorage-backed) with route guards
- Full CRUD todo management with filtering (all/active/completed)
- 119+ Jasmine/Karma unit tests covering services, components, guards, and state

## Rules

| File pattern | Rules applied |
|---|---|
| `**/*.cs` | `rules/csharp/` (10 files) + `rules/common/` (7 files) |
| `**/*.ts`, `**/*.html` | `rules/typescript/` (7 files) + `rules/common/` (7 files) |

Key topics: modularization, domain modeling, persistence, services, presentation, hosting, testing, coding style, CSS architecture, Angular patterns, security, command-line.

## Getting started

```bash
git clone https://github.com/vgaytanovcorvel/claude-clean-architecture-mistery
cd claude-clean-architecture-mistery

# Build the backend
dotnet build MisteryApp.slnx

# Run backend tests
dotnet test MisteryApp.slnx

# Run the API
dotnet run --project src/MisteryApp.Web.Api

# Install Angular dependencies and run frontend tests
cd src/misteryapp.web.client.angular
npm install
ng test --browsers=ChromeHeadlessNoSandbox

# Run the full SPA + API together
cd ../..
dotnet run --project src/MisteryApp.Web.Server
```

Then describe what you want to build and Claude Code will follow the clean architecture rules automatically.

## Key conventions

### Backend (C#)

| Concern | Convention |
|---|---|
| Validation | `FluentValidation` — no Data Annotations on records |
| Time | Inject `TimeProvider`, never `DateTime.UtcNow` |
| Repositories | `IDbContextFactory<T>`, method names prefixed by entity (`UserSingleByIdAsync`) |
| API responses | `ApiResponse<T>` envelope on all endpoints |
| Testing | `Mock<SUT>` (strict), `VerifyAll()`, `FakeTimeProvider` |
| Virtual methods | All public/internal service and repository methods must be `virtual` |

### Frontend (Angular)

| Concern | Convention |
|---|---|
| Components | Standalone, `OnPush`, external templates, signal inputs/outputs |
| State | `signal()` / `computed()` / `resource()` — no NgRx |
| Architecture | Domain -> Repository -> Service -> State -> Presentation |
| Styling | SCSS with ITCSS layers, CSS custom properties (design tokens) |
| Testing | Jasmine + Karma, AAA pattern, `describe`/`it` naming |

## Branches

| Branch | Description |
|---|---|
| `main` | Bare clean architecture baseline (no domain logic) |
| `todo-angular-v2` | Multi-user Todo app with Angular 19 glassmorphism SPA + full test suite |
| `todo-angular` | Earlier iteration of the Angular Todo app |
| `todo-react` | React-based Todo app variant |

## Requirements

- .NET 9 SDK
- Node.js 18+ and npm (for Angular SPA)
- SQL Server / LocalDB (for EF Core migrations)
- [GitHub CLI](https://cli.github.com) + [Claude Code](https://claude.ai/code) (optional but recommended)
