# testmaker_net

Backend solution for the testmaker domain, organized as a multi-project .NET 8 solution with Domain, Application, Infrastructure, and API layers.

This README documents two things separately:

1. The architecture that exists in the repository today.
2. The intended target architecture discussed during project design.

That split is deliberate. The codebase already has a solid persistence foundation, but the CQRS and vertical-slice application flow is still being built out.

## Architecture Overview

The solution currently follows a layered structure:

- `testmaker.Domain`: core entity model.
- `testmaker.Application`: placeholder application layer for future CQRS handlers and shared application concerns.
- `testmaker.Infrastructure`: EF Core persistence, database configuration, and infrastructure DI registration.
- `testmaker.Api`: ASP.NET Core host, Swagger, and top-level HTTP pipeline.

The long-term direction is a cleaner CQRS-based structure where the API project contains endpoints only, the Application project contains commands, queries, handlers, and validators, the Domain project contains entities and core rules, and Infrastructure implements persistence and external services.

## Current Solution Structure

```text
testmaker_net/
├── testmaker.Api/
│   ├── Features/
│   ├── Properties/
│   ├── Program.cs
│   ├── appsettings.json
│   └── testmaker.Api.csproj
├── testmaker.Application/
│   ├── Common/
│   │   └── Interfaces/
│   ├── Features/
│   └── testmaker.Application.csproj
├── testmaker.Domain/
│   ├── Common/
│   ├── Entities/
│   └── testmaker.Domain.csproj
├── testmaker.Infrastructure/
│   ├── Interfaces/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── ApplicationDbContext.cs
│   ├── DependencyInjection.cs
│   └── testmaker.Infrastructure.csproj
├── Directory.Packages.props
└── testmaker_net.sln
```

## Current Architecture

### Domain

`testmaker.Domain` is the most complete layer today. It contains the entity model for the core testmaker concepts, including:

- schools, classes, and subjects
- tests and test types
- question details, question types, difficulty, and images
- mapping entities such as test-question, test-section, and subquestion mappings

These entities are represented as plain domain classes and are currently the main source of business data structure.

### Application

`testmaker.Application` exists as the intended home for application use cases, but it is still only lightly implemented. The `Common` and `Features` folders are present, and the repository now defines the `IApplicationDbContext` abstraction there, but it does not yet contain command handlers, query handlers, validators, or broader application wiring.

Today, this means the Application layer describes architectural intent more than current behavior.

### Infrastructure

`testmaker.Infrastructure` is where the current implementation is strongest:

- `Persistence/ApplicationDbContext.cs` defines the EF Core `DbContext`
- `Persistence/Configurations/*` contains entity configuration classes
- `DependencyInjection.cs` registers the database context and the persistence abstraction
- `Application` now defines `Common/Interfaces/IApplicationDbContext.cs`, which Infrastructure implements

Infrastructure uses EF Core with Pomelo for MySQL and central package management through `Directory.Packages.props`.

### API

`testmaker.Api` is the ASP.NET Core host. It currently provides:

- controller registration through `AddControllers()`
- OpenAPI/Swagger registration
- MediatR registration
- the main HTTP pipeline in `Program.cs`

At the moment, the API project is mostly host setup. There are no controller files or feature endpoint implementations in the repository yet.

## Dependency Direction

### Current project references

The code currently compiles with this dependency flow:

```text
testmaker.Api -> testmaker.Application
testmaker.Application -> testmaker.Domain
testmaker.Infrastructure -> testmaker.Domain
testmaker.Infrastructure -> testmaker.Application
testmaker.Domain -> no project references
```

This is close to the intended inward dependency model, but not identical.

### Important current mismatch

The persistence abstraction, `IApplicationDbContext`, now lives in `testmaker.Application/Common/Interfaces`, which is the right direction for Clean Architecture. The repository still does not fully match the target architecture, though, because the API host is not yet wired to call the Infrastructure registration, and MediatR is still scanning the API assembly rather than the Application assembly.

That distinction matters for the README because the repository should not be described as fully following the target architecture yet.

## Persistence Architecture

The database stack is already well-defined.

### EF Core and MySQL

The solution uses:

- EF Core 8
- Pomelo EntityFrameworkCore MySql
- a central package-management file: `Directory.Packages.props`

`ApplicationDbContext` configures the persistence model with a few notable conventions:

- all entity configurations are applied automatically from the Infrastructure assembly
- all `Guid` properties are converted to strings with max length 36
- the model uses `utf8mb4` charset and `utf8mb4_0900_ai_ci` collation

This means the database layer is already structured around explicit EF Core configurations rather than data annotations scattered across entity classes.

### Entity configuration pattern

Each entity has a dedicated configuration class under `testmaker.Infrastructure/Persistence/Configurations`. This keeps database concerns isolated from the domain classes and is consistent with a clean layered design.

### Current DbContext abstraction

`DependencyInjection.cs` registers `ApplicationDbContext` and maps it to the `IApplicationDbContext` contract defined in Application. That part now aligns with the intended dependency direction, but the API host does not yet call `AddInfrastructure(builder.Configuration)`.

As a result, the repository has the infrastructure wiring defined, but not yet connected into the running host pipeline.

## Intended Target Architecture

The original design discussion for this project points toward a stricter Clean Architecture and CQRS approach.

### Target responsibilities

- `testmaker.Api`: endpoints only, request transport, middleware, and startup wiring
- `testmaker.Application`: commands, queries, handlers, validators, behaviors, and application interfaces
- `testmaker.Domain`: entities and core business model
- `testmaker.Infrastructure`: EF Core, database access, and implementations of application-defined interfaces

### Target feature organization

The intended structure is vertical by feature inside the Application layer, for example:

```text
testmaker.Application/
├── Features/
│   ├── Tests/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Questions/
│   │   ├── Commands/
│   │   └── Queries/
│   └── ...
└── Common/
```

In that model:

- API endpoints remain thin
- MediatR dispatches commands and queries into the Application layer
- validation behaviors run centrally
- Infrastructure stays behind abstractions defined by Application

This target architecture is not fully implemented yet and should be treated as the next structural milestone, not the current state.

## Current Implementation Status

The repository is best understood as a foundation in progress.

### Already implemented

- solution split into four projects
- domain entity model
- EF Core `DbContext`
- entity configurations for the current schema
- MySQL provider setup in Infrastructure
- Swagger and basic ASP.NET Core host setup
- central package management

### Not implemented or not wired yet

- application command/query handlers
- validators and pipeline behaviors in the Application layer
- API controllers or feature endpoints
- Infrastructure registration from the API startup path
- MediatR scanning of the Application assembly

### Why this matters for contributors

New contributors should treat the current codebase as an infrastructure-first backend skeleton. Persistence and schema mapping are the most concrete parts of the system right now. The request-handling architecture described in the target section is still a work in progress.

## Local Configuration

### Requirements

- .NET 8 SDK
- MySQL server accessible from the local machine

### Connection string

The API project reads the database connection string from `testmaker.Api/appsettings.json` under:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Server=localhost;Database=testmaker_v2;User=<user>;Password=<password>;"
	}
}
```

Use local development credentials appropriate to your environment. Avoid committing real credentials into documentation or shared config.

### Launch profiles

The API project includes two local launch profiles:

- HTTP: `http://localhost:5216`
- HTTPS: `https://localhost:7294`

## Development Notes

### Current startup behavior

`Program.cs` currently registers controllers, Swagger, and MediatR, but it does not yet invoke the Infrastructure extension method. If you begin implementing request handlers or database-backed endpoints, wiring `AddInfrastructure(builder.Configuration)` into startup is one of the first follow-up tasks.

### Package management

NuGet package versions are managed centrally through `Directory.Packages.props`. Project files reference packages without repeating version numbers.

## Recommended Next Architecture Steps

1. Add `DependencyInjection.cs` to `testmaker.Application` for MediatR, validators, and pipeline behaviors.
2. Call `AddInfrastructure(builder.Configuration)` and `AddApplication()` from the API startup path.
3. Change MediatR registration to scan the Application assembly instead of the API assembly.
4. Start introducing feature-based commands and queries in `testmaker.Application/Features`.
5. Keep the API layer limited to endpoints, transport concerns, and middleware as features are added.

## Summary

This repository is currently a layered .NET backend with a strong EF Core persistence base and a planned move toward CQRS with vertical slices. The README should be read with that split in mind: current implementation first, target architecture second.