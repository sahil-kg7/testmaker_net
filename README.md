# testmaker_net

Backend solution for the testmaker domain, organized as a multi-project .NET 8 solution with Domain, Application, Infrastructure, and API layers.

This README documents two things separately:

1. The architecture that exists in the repository today.
2. The intended target architecture discussed during project design.

That split is deliberate. The codebase now has working startup wiring, MediatR-based request handling, validation behavior, exception middleware, and a first vertical slice for Classes, while the broader domain is still being migrated into the same pattern.

## Architecture Overview

The solution currently follows a layered structure:

- `testmaker.Domain`: core entity model.
- `testmaker.Application`: commands, queries, handlers, validators, behaviors, shared result types, and application abstractions.
- `testmaker.Infrastructure`: EF Core persistence, database configuration, and infrastructure DI registration.
- `testmaker.Api`: ASP.NET Core host, controllers, middleware, Swagger, and top-level HTTP pipeline.

The long-term direction remains a cleaner CQRS-based structure where the API project contains endpoints only, the Application project contains commands, queries, handlers, and validators organized by feature, the Domain project contains entities and core rules, and Infrastructure implements persistence and external services.

## Current Solution Structure

```text
testmaker_net/
├── testmaker.Api/
│   ├── Features/
│   │   └── Classes/
│   │       └── ClassesController.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Properties/
│   ├── Program.cs
│   ├── appsettings.json
│   └── testmaker.Api.csproj
├── testmaker.Application/
│   ├── Common/
│   │   ├── Behaviors/
│   │   │   └── ValidationBehavior.cs
│   │   ├── Interfaces/
│   │   │   └── IApplicationDbContext.cs
│   │   └── Result.cs
│   ├── Features/
│   │   └── Classes/
│   │       ├── Commands/
│   │       └── Queries/
│   ├── DependencyInjection.cs
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

These entities are represented as plain domain classes and remain the main source of business data structure.

### Application

`testmaker.Application` is now active and no longer only an architectural placeholder. It currently contains:

- `Common/Interfaces/IApplicationDbContext.cs` as the persistence abstraction used by handlers
- `Common/Behaviors/ValidationBehavior.cs` as a MediatR pipeline behavior that runs FluentValidation validators before handlers execute
- `Common/Result.cs` with `Result` and `Result<T>` for expected business failures
- `DependencyInjection.cs` with `AddApplication()` for registering MediatR, validators, and pipeline behaviors
- the first feature slice under `Features/Classes/`

The current error-handling model is hybrid by design:

- expected business failures are returned through `Result` / `Result<T>`
- validation failures are thrown as `ValidationException` from the validation behavior
- unexpected runtime failures bubble out to API middleware

`IApplicationDbContext` now exposes both entity sets and `SaveChangesAsync`, so handlers can perform persistence through the Application-defined abstraction instead of depending on Infrastructure types.

### Infrastructure

`testmaker.Infrastructure` remains the strongest persistence-focused layer. It provides:

- `Persistence/ApplicationDbContext.cs` as the EF Core `DbContext`
- `Persistence/Configurations/*` as entity configuration classes
- `DependencyInjection.cs` with `AddInfrastructure(IConfiguration)`
- the implementation behind the `IApplicationDbContext` abstraction defined in Application

Infrastructure uses EF Core with Pomelo for MySQL and central package management through `Directory.Packages.props`.

### API

`testmaker.Api` is now more than host setup. It currently provides:

- controller registration through `AddControllers()`
- OpenAPI/Swagger registration
- startup wiring for `AddApplication()` and `AddInfrastructure(builder.Configuration)`
- global exception handling through `ExceptionHandlingMiddleware`
- the first feature controller: `Features/Classes/ClassesController.cs`

The current API layer is still intentionally thin. Controllers dispatch commands and queries through `ISender` and translate `Result` failures into HTTP responses, while middleware handles validation and unexpected exceptions centrally.

## Dependency Direction

### Current project references

The code currently compiles with this dependency flow:

```text
testmaker.Api -> testmaker.Application
testmaker.Api -> testmaker.Infrastructure
testmaker.Application -> testmaker.Domain
testmaker.Infrastructure -> testmaker.Domain
testmaker.Infrastructure -> testmaker.Application
testmaker.Domain -> no project references
```

This is close to the intended inward dependency model, though the API project still directly references Infrastructure for startup wiring.

### Current gap versus target architecture

The main remaining gap is no longer startup wiring. The repository now correctly wires both Application and Infrastructure from the API host, and MediatR is registered from the Application assembly.

The remaining gap is breadth rather than wiring:

- only the `Classes` feature has been implemented as a vertical slice
- most of the domain still does not yet have commands, queries, handlers, validators, or endpoints
- the API still mixes transport mapping with `Result` interpretation in controllers rather than using a broader shared response-mapping abstraction

That distinction matters because the repository should now be described as an early CQRS foundation with one implemented slice, not as an unwired skeleton.

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

`DependencyInjection.cs` in Infrastructure registers `ApplicationDbContext` and maps it to the `IApplicationDbContext` contract defined in Application. `Program.cs` in the API host now calls `AddInfrastructure(builder.Configuration)`, so the infrastructure wiring is no longer just defined; it is part of the running host pipeline.

## Request Flow and Error Handling

The current request flow looks like this:

1. An HTTP request enters `testmaker.Api`.
2. `ExceptionHandlingMiddleware` wraps the downstream pipeline.
3. A controller action sends a command or query through MediatR.
4. `ValidationBehavior<TRequest, TResponse>` runs any registered FluentValidation validators.
5. If validation fails, a `ValidationException` is thrown and converted by middleware into a `400 Bad Request` response.
6. If validation passes, the handler runs.
7. Expected business failures are returned as `Result` / `Result<T>`.
8. Controllers translate those expected failures into HTTP responses such as `404` or `409`.
9. Unexpected runtime exceptions bubble to the middleware and are converted into a generic `500 Internal Server Error` response.

This gives the repository a clear split:

- `Result` for expected business outcomes
- exceptions plus middleware for validation and unexpected failures

## Current Feature Coverage

The first implemented vertical slice is `Classes`.

### Application-side feature implementation

`testmaker.Application/Features/Classes` currently contains:

- queries for listing classes and retrieving a class by number
- commands for creating, updating, and deleting classes
- handlers for each command and query
- validators for create and update operations
- a small DTO for returning class data

### API-side feature implementation

`testmaker.Api/Features/Classes/ClassesController.cs` currently exposes:

- `GET /api/classes`
- `GET /api/classes/{classNumber}`
- `POST /api/classes`
- `PUT /api/classes/{classNumber}`
- `DELETE /api/classes/{classNumber}`

This controller is the current reference implementation for how future features should be added.

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

This target architecture is now partially implemented. The `Classes` slice follows the intended direction, but most of the domain has not yet been migrated into that same structure.

## Current Implementation Status

The repository is best understood as an early but functioning CQRS foundation.

### Already implemented

- solution split into four projects
- domain entity model
- EF Core `DbContext`
- entity configurations for the current schema
- MySQL provider setup in Infrastructure
- central package management
- Application DI registration through `AddApplication()`
- MediatR scanning of the Application assembly
- FluentValidation validator registration
- validation pipeline behavior in the Application layer
- `IApplicationDbContext` abstraction with `SaveChangesAsync`
- Infrastructure registration from the API startup path
- global exception middleware in the API layer
- first feature slice for `Classes`
- first API controller and endpoints for `Classes`

### Not implemented or still limited

- additional feature slices beyond `Classes`
- endpoints for schools, subjects, tests, questions, and related mappings
- automated tests for handlers, validators, middleware, and controllers
- a broader shared convention for translating `Result` values to HTTP responses across all controllers

### Why this matters for contributors

New contributors should treat the current codebase as a working backend foundation rather than only a persistence skeleton. The core request flow is now established, and the `Classes` feature should be used as the reference pattern for future slices.

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

`Program.cs` now registers controllers, Swagger, `AddApplication()`, `AddInfrastructure(builder.Configuration)`, and `ExceptionHandlingMiddleware`. This means the full request path from HTTP endpoint to handler to persistence is wired and runnable.

### Current error-handling convention

The repository currently uses this convention:

- expected business failures use `Result` / `Result<T>`
- validation failures are thrown from `ValidationBehavior`
- unexpected runtime failures are handled by API middleware

Contributors should follow that pattern unless the project intentionally decides to standardize on a different model later.

### Package management

NuGet package versions are managed centrally through `Directory.Packages.props`. Project files reference packages without repeating version numbers.

## Recommended Next Architecture Steps

1. Add more feature slices such as `Schools`, `Subjects`, and `Tests` using the same Application-plus-API pattern as `Classes`.
2. Add automated tests for validators, handlers, middleware, and controller behavior.
3. Introduce a broader shared convention for translating `Result` failures into HTTP responses to reduce repeated controller mapping logic.
4. Extend the README and contributor guidance as additional feature slices are added so the reference pattern stays current.
5. Keep the API layer limited to transport concerns, middleware, and endpoint orchestration as more features are added.

## Summary

This repository is currently a layered .NET backend with a working CQRS foundation, central validation, infrastructure wiring, exception middleware, and an initial vertical slice for `Classes`. The target architecture is no longer just aspirational, but it is still only partially implemented across the broader testmaker domain.