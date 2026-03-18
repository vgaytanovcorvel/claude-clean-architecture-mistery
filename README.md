# MisteryApp — Clean Architecture Baseline for Claude Code

A pre-scaffolded C# clean architecture solution you can clone and immediately start building with [Claude Code](https://claude.ai/code), without spending time on project setup or architecture decisions.

## What's included

- **8 source projects** wired with correct dependency flow
- **8 test projects** (MSTest + Moq + FluentAssertions)
- **Central package management** via `Directory.Packages.props`
- **Per-module `CLAUDE.md` files** — Claude Code reads these automatically to understand each layer's rules
- **22 architecture rule files** under `rules/` covering coding style, persistence, testing, security, and more

## Architecture

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

Dependency flow: `Common ← Abstractions ← Implementation/Repository ← Web.Core ← Web.Api/Web.Server/Cli`

## Getting started

```bash
git clone https://github.com/vgaytanovcorvel/claude-clean-architecture-mistery
cd claude-clean-architecture-mistery

# Verify it builds
dotnet build MisteryApp.slnx

# Open in your editor and start Claude Code
code .
```

Then describe what you want to build and Claude Code will follow the clean architecture rules automatically.

## Example feature: Unstructured Ingestor

The `feature/unstructured-ingestor` branch adds an `ingest` CLI command that processes a folder of mixed-format files (raw logs, CSVs, OCR text) through a 4-agent AI pipeline into a unified SQL Server schema.

```bash
# Dry-run (no DB write)
dotnet run --project src/MisteryApp.Cli -- ingest ./samples --dry-run

# Full run with format override
export MISTERYAPP_OPENAI__APIKEY=sk-...
dotnet run --project src/MisteryApp.Cli -- ingest ./samples --format csv
```

**Pipeline:** `IngestDispatcher` → `SchemaMapper` → `QualityCritic` → `HealerAgent`

Each agent is backed by Semantic Kernel (`Microsoft.SemanticKernel`) prompt invocation via a thin `IKernelInvoker` shim (makes agents Moq-testable without touching the real `Kernel`). `CsvHelper` is used for structured CSV parsing before the mapping prompt.

After a successful run, records are batch-inserted into `IngestedRecords` via `IIngestedRecordRepository`. Apply the EF migration to create the table:

```bash
dotnet ef migrations add AddIngestedRecords --project src/MisteryApp.Repository
dotnet ef database update --project src/MisteryApp.Repository
```

## Key conventions

| Concern | Convention |
|---|---|
| Validation | `FluentValidation` — no Data Annotations on records |
| Time | Inject `TimeProvider`, never `DateTime.UtcNow` |
| Repositories | `IDbContextFactory<T>`, method names prefixed by entity (`UserSingleByIdAsync`) |
| API responses | `ApiResponse<T>` envelope on all endpoints |
| Testing | `Mock<SUT>` (strict), `VerifyAll()`, `FakeTimeProvider` |
| Virtual methods | All public/internal service and repository methods must be `virtual` |

## Requirements

- .NET 9 SDK
- SQL Server / LocalDB (for EF Core migrations)
- OpenAI API key (`MISTERYAPP_OPENAI__APIKEY`) — required for the ingest pipeline
- [GitHub CLI](https://cli.github.com) + [Claude Code](https://claude.ai/code) (optional but recommended)
