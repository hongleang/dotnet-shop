# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Status

Freshly scaffolded .NET 10 solution (`DotnetShop.slnx`, the XML solution format). Most projects still contain template placeholders (`Class1.cs`, `UnitTest1.cs`, the weather-forecast endpoint in `Program.cs`). The architecture below reflects the intent encoded in project references and NuGet packages; replace placeholders as real code lands.

## Commands

```bash
dotnet build                                   # build the whole solution
dotnet run --project src/DotnetShop.Api        # run the API (http://localhost:5271, https://localhost:7176 with --launch-profile https)
dotnet test                                    # run all tests
dotnet test tests/DotnetShop.UnitTests         # one test project
dotnet test --filter "FullyQualifiedName~Namespace.ClassName.MethodName"   # single test
dotnet format                                  # format / lint
```

OpenAPI document is served at `/openapi/v1.json` in Development. `src/DotnetShop.Api/DotnetShop.Api.http` holds sample requests.

EF Core migrations (DbContext belongs in Infrastructure; the Api project carries `Microsoft.EntityFrameworkCore.Design`, so it is the startup project). Requires the `dotnet-ef` tool:

```bash
dotnet ef migrations add <Name> --project src/DotnetShop.Infrastructure --startup-project src/DotnetShop.Api
dotnet ef database update      --project src/DotnetShop.Infrastructure --startup-project src/DotnetShop.Api
```

Integration tests use Testcontainers (PostgreSQL), so Docker must be running.

## Architecture

Clean Architecture with dependencies pointing inward:

```
Api ──► Application ──► Domain
 └────► Infrastructure ──► Application
```

- **Domain** — entities and domain logic; no package dependencies. Keep it that way.
- **Application** — use cases via **MediatR** (commands/queries + handlers) with **FluentValidation** validators. References EF Core directly (not just abstractions), which suggests handlers work against a DbContext interface (e.g. `IApplicationDbContext`) defined here and implemented in Infrastructure, rather than repositories.
- **Infrastructure** — EF Core with **Npgsql** (PostgreSQL) and **ASP.NET Core Identity** (`Identity.EntityFrameworkCore`). Implements Application interfaces.
- **Api** — ASP.NET Core minimal API host (`Program.cs` with top-level statements). Composition root: wires Application + Infrastructure DI, **JWT bearer** authentication, **Serilog** logging, and OpenAPI.

Tests:
- **UnitTests** — xUnit; references Domain and Application only (no infrastructure/DB).
- **IntegrationTests** — xUnit + `WebApplicationFactory<Program>` (`Microsoft.AspNetCore.Mvc.Testing`) against a real Postgres container via Testcontainers; references Api.

All projects target `net10.0` with `Nullable` and `ImplicitUsings` enabled.
