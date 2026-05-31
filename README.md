# testmaker_net

Backend solution for the testmaker domain, organized as a multi-project .NET 8 solution with Domain, Application, Infrastructure, and API layers.

This README documents two things separately:

1. The architecture that exists in the repository today.
2. The intended target architecture discussed during project design.

That split is deliberate. The codebase now has working startup wiring, MediatR-based request handling, validation behavior, exception middleware, and five complete vertical slices for Classes, Schools, Subjects, Questions, and Tests.

## Architecture Overview

The solution currently follows a layered structure:

- `testmaker.Domain`: core entity model.
- `testmaker.Application`: commands, queries, handlers, validators, behaviors, shared result types, and application abstractions.
- `testmaker.Infrastructure`: EF Core persistence, database configuration, and infrastructure DI registration.
- `testmaker.Api`: ASP.NET Core host, endpoints, middleware, Swagger, and top-level HTTP pipeline.

The long-term direction remains a cleaner CQRS-based structure where the API project contains endpoints only, the Application project contains commands, queries, handlers, and validators organized by feature, the Domain project contains entities and core rules, and Infrastructure implements persistence and external services.

## Current Solution Structure

```text
testmaker_net/
├── testmaker.Api/
│   ├── Common/
│   │   ├── ApiResult.cs
│   │   └── Models/
│   │       ├── UpsertQuestionRequest.cs      # API-layer transport DTO
│   │       └── UpsertTestRequest.cs          # API-layer transport DTO
│   ├── Features/
│   │   ├── Classes/
│   │   │   ├── Endpoints/
│   │   │   │   ├── Classes.Create.cs
│   │   │   │   ├── Classes.Delete.cs
│   │   │   │   ├── Classes.GetAll.cs
│   │   │   │   ├── Classes.GetById.cs
│   │   │   │   └── Classes.Update.cs
│   │   │   ├── Models/
│   │   │   │   ├── CreateClassRequest.cs
│   │   │   │   └── UpdateClassRequest.cs
│   │   │   └── ClassEndpoints.cs
│   │   ├── Schools/
│   │   │   ├── Endpoints/
│   │   │   │   ├── Schools.Create.cs
│   │   │   │   ├── Schools.Delete.cs
│   │   │   │   ├── Schools.GetAll.cs
│   │   │   │   ├── Schools.GetById.cs
│   │   │   │   └── Schools.Update.cs
│   │   │   ├── Models/
│   │   │   │   ├── CreateSchoolRequest.cs
│   │   │   │   └── UpdateSchoolRequest.cs
│   │   │   └── SchoolEndpoints.cs
│   │   ├── Subjects/
│   │   │   ├── Endpoints/
│   │   │   │   ├── Subjects.Create.cs
│   │   │   │   ├── Subjects.Delete.cs
│   │   │   │   ├── Subjects.GetAll.cs
│   │   │   │   ├── Subjects.GetById.cs
│   │   │   │   └── Subjects.Update.cs
│   │   │   ├── Models/
│   │   │   │   ├── CreateSubjectRequest.cs
│   │   │   │   └── UpdateSubjectRequest.cs
│   │   │   └── SubjectEndpoints.cs
│   │   ├── Questions/
│   │   │   ├── Endpoints/
│   │   │   │   ├── Questions.Create.cs
│   │   │   │   ├── Questions.Delete.cs
│   │   │   │   ├── Questions.GetAll.cs
│   │   │   │   ├── Questions.GetById.cs
│   │   │   │   └── Questions.Update.cs
│   │   │   └── QuestionEndpoints.cs
│   │   ├── Tests/
│   │   │   ├── Endpoints/
│   │   │   │   ├── Tests.Create.cs
│   │   │   │   ├── Tests.Delete.cs
│   │   │   │   ├── Tests.GetAll.cs
│   │   │   │   ├── Tests.GetById.cs
│   │   │   │   └── Tests.Update.cs
│   │   │   ├── Models/
│   │   │   │   └── UpsertTestRequest.cs
│   │   │   └── TestEndpoints.cs
│   │   ├── QuestionTypes/
│   │   │   ├── Endpoints/
│   │   │   │   └── QuestionTypes.GetAll.cs
│   │   │   └── QuestionTypeEndpoints.cs
│   │   ├── QuestionDifficulties/
│   │   │   ├── Endpoints/
│   │   │   │   └── QuestionDifficulties.GetAll.cs
│   │   │   └── QuestionDifficultyEndpoints.cs
│   │   └── TestTypes/
│   │       ├── Endpoints/
│   │       │   └── TestTypes.GetAll.cs
│   │       └── TestTypeEndpoints.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Program.cs
│   └── testmaker.Api.csproj
├── testmaker.Application/
│   ├── Common/
│   │   ├── Behaviors/
│   │   │   └── ValidationBehavior.cs
│   │   ├── Interfaces/
│   │   │   └── IApplicationDbContext.cs
│   │   └── Result.cs
│   ├── Features/
│   │   ├── Classes/
│   │   │   ├── Commands/
│   │   │   ├── Contracts/
│   │   │   └── Queries/
│   │   ├── Schools/
│   │   │   ├── Commands/
│   │   │   ├── Contracts/
│   │   │   └── Queries/
│   │   ├── Subjects/
│   │   │   ├── Commands/
│   │   │   ├── Contracts/
│   │   │   └── Queries/
│   │   ├── Questions/
│   │   │   ├── Commands/
│   │   │   ├── Common/
│   │   │   ├── Contracts/
│   │   │   └── Queries/
│   │   ├── Tests/
│   │   │   ├── Commands/
│   │   │   ├── Common/
│   │   │   ├── Contracts/
│   │   │   └── Queries/
│   │   ├── QuestionTypes/
│   │   │   └── Queries/
│   │   ├── QuestionDifficulties/
│   │   │   └── Queries/
│   │   └── TestTypes/
│   │       └── Queries/
│   ├── DependencyInjection.cs
│   └── testmaker.Application.csproj
├── testmaker.Domain/
│   ├── Entities/
│   └── testmaker.Domain.csproj
├── testmaker.Infrastructure/
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
- feature slices under `Features/` organized by domain concept: `Classes`, `Schools`, `Subjects`, `Questions`, `Tests`, `QuestionTypes`, `QuestionDifficulties`, `TestTypes`

Each feature slice follows a consistent structure separating data shapes from business logic:

- **Contracts/** — pure record types defining DTOs (request and response types). No logic.
- **Common/** — business logic shared across commands/queries: mapper, validators, enums.
- **Commands/** — write operations (Create, Update, Delete).
- **Queries/** — read operations (GetAll, GetById).

This separation enables the API layer to remain thin: it only translates between HTTP transport DTOs and Application-layer contracts.

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
- feature endpoints organized under `Features/{Feature}/Endpoints/`

The API layer is intentionally thin. Each endpoint:

1. Deserializes the HTTP request into an API-layer request DTO
2. Maps it to an Application-layer command/query
3. Sends via MediatR
4. Maps the `Result` back to an HTTP response

**Key principle:** API-layer request DTOs handle HTTP transport concerns (serialization, naming conventions). Application-layer contracts handle business logic concerns (what the handler needs).

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

The startup wiring gap is resolved. The repository now correctly wires both Application and Infrastructure from the API host, and MediatR is registered from the Application assembly.

Five feature slices (`Classes`, `Schools`, `Subjects`, `Questions`, `Tests`) are fully implemented with get-all, get-by-id, create, update, and delete. Three additional features (`QuestionTypes`, `QuestionDifficulties`, `TestTypes`) have read-only endpoints. The Contract separation pattern (`Contracts/` for DTOs, `Common/` for business logic) is established as the standard approach.

The remaining gap is breadth:

- automated tests for handlers, validators, middleware, and endpoints are not yet in place
- a broader shared convention for translating `Result` values to HTTP responses across all endpoints

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

Eight feature slices exist: `Classes`, `Schools`, `Subjects`, `Questions`, `Tests`, `QuestionTypes`, `QuestionDifficulties`, and `TestTypes`.

### Classes feature

**Application layer** (`testmaker.Application/Features/Classes`):
- `GetAllClassesQuery` / handler — returns all classes ordered by name
- `GetClassByIdQuery` / handler — returns a single class
- `CreateClassCommand` / handler — creates a new class
- `UpdateClassCommand` / handler — updates an existing class
- `DeleteClassCommand` / handler — deletes a class
- `CreateClassCommandValidator` — validates ClassName is not empty

**API layer** (`testmaker.Api/Features/Classes/Endpoints/`):
- `GET /api/classes` — list all classes
- `GET /api/classes/{id}` — get class by ID
- `POST /api/classes` — create a new class
- `PUT /api/classes/{id}` — update an existing class
- `DELETE /api/classes/{id}` — delete a class

### Schools feature

**Application layer** (`testmaker.Application/Features/Schools`):
- `GetAllSchoolsQuery` / handler — returns all schools ordered by name
- `GetSchoolByIdQuery` / handler — returns a single school
- `CreateSchoolCommand` / handler — creates a new school
- `UpdateSchoolCommand` / handler — updates an existing school
- `DeleteSchoolCommand` / handler — deletes a school

**API layer** (`testmaker.Api/Features/Schools/Endpoints/`):
- `GET /api/schools` — list all schools
- `GET /api/schools/{id}` — get school by ID
- `POST /api/schools` — create a new school
- `PUT /api/schools/{id}` — update an existing school
- `DELETE /api/schools/{id}` — delete a school

### Subjects feature

**Application layer** (`testmaker.Application/Features/Subjects`):
- `GetAllSubjectsQuery` / handler — returns all subjects ordered by name
- `GetSubjectByIdQuery` / handler — returns a single subject
- `CreateSubjectCommand` / handler — creates a new subject
- `UpdateSubjectCommand` / handler — updates an existing subject
- `DeleteSubjectCommand` / handler — deletes a subject

**API layer** (`testmaker.Api/Features/Subjects/Endpoints/`):
- `GET /api/subjects` — list all subjects
- `GET /api/subjects/{id}` — get subject by ID
- `POST /api/subjects` — create a new subject
- `PUT /api/subjects/{id}` — update an existing subject
- `DELETE /api/subjects/{id}` — delete a subject

### Questions feature

**Application layer** (`testmaker.Application/Features/Questions`):
- `GetAllQuestionsQuery` / handler — returns questions with filtering
- `GetQuestionByIdQuery` / handler — returns a single question with images
- `CreateQuestionCommand` / handler — creates a new question with images
- `UpdateQuestionCommand` / handler — updates an existing question
- `DeleteQuestionCommand` / handler — deletes a question
- `QuestionValidator` — validates references (type, difficulty, class, subject)
- `QuestionMapper` — entity ↔ DTO mapping
- `QuestionRequestValidator` — FluentValidation for the request DTO
- `Contracts/` — `QuestionRequest`, `QuestionDto`, `QuestionListItemDto`, `QuestionImageRequest`, `QuestionImageDto`

**API layer** (`testmaker.Api/Features/Questions/Endpoints/`):
- `GET /api/questions` — list questions with optional filters
- `GET /api/questions/{id}` — get question by ID
- `POST /api/questions` — create a new question
- `PUT /api/questions/{id}` — update an existing question
- `DELETE /api/questions/{id}` — delete a question

### Tests feature

**Application layer** (`testmaker.Application/Features/Tests`):
- `GetAllTestsQuery` / handler — returns paginated tests with optional filters
- `GetTestByIdQuery` / handler — returns a single test with questions
- `CreateTestCommand` / handler — creates a new test with inline or existing questions
- `UpdateTestCommand` / handler — updates an existing test
- `DeleteTestCommand` / handler — deletes a test
- `TestValidator` — validates references (school, class, subject, test type) and sections
- `TestMapper` — loads test detail with questions and subquestions
- `TestAssemblyBuilder` — populates test-question and subquestion maps
- `Contracts/` — `TestDetailDto`, `TestListItemDto`, `TestQuestionBriefDto`, `TestSubquestionBriefDto`, `TestQuestionInput`, `TestSubquestionInput`

**API layer** (`testmaker.Api/Features/Tests/Endpoints/`):
- `GET /api/tests` — list tests with optional filters and pagination
- `GET /api/tests/{id}` — get test by ID
- `POST /api/tests` — create a new test
- `PUT /api/tests/{id}` — update an existing test
- `DELETE /api/tests/{id}` — delete a test

### QuestionTypes, QuestionDifficulties, TestTypes features

These three features currently have read-only endpoints (GetAll only):

**API layer:**
- `GET /api/questiontypes` — list all question types
- `GET /api/questiondifficulties` — list all question difficulties
- `GET /api/testtypes` — list all test types

## Intended Target Architecture

The original design discussion for this project points toward a stricter Clean Architecture and CQRS approach.

### Target responsibilities

- `testmaker.Api`: endpoints only, request transport, middleware, and startup wiring
- `testmaker.Application`: commands, queries, handlers, validators, behaviors, and application interfaces
- `testmaker.Domain`: entities and core business model
- `testmaker.Infrastructure`: EF Core, database access, and implementations of application-defined interfaces

### Target feature organization

The intended structure is vertical by feature inside the Application layer, separating data shapes from business logic:

```text
testmaker.Application/Features/{Feature}/
├── Commands/                    # Write operations
│   └── {Operation}/
│       ├── {Operation}Command.cs
│       ├── {Operation}CommandHandler.cs
│       └── {Operation}CommandValidator.cs
├── Queries/                     # Read operations
│   └── {Query}/
│       ├── {Query}Query.cs
│       └── {Query}QueryHandler.cs
├── Contracts/                   # Pure data shapes (DTOs)
│   ├── {Entity}Request.cs       # Input DTO
│   └── {Entity}Dto.cs           # Output DTO
└── Common/                      # Shared business logic
    ├── {Entity}Mapper.cs        # Entity ↔ DTO mapping
    └── {Entity}Validator.cs     # Reference validation
```

In that model:

- API endpoints remain thin
- MediatR dispatches commands and queries into the Application layer
- validation behaviors run centrally
- Infrastructure stays behind abstractions defined by Application

This target architecture is now partially implemented. The `Classes`, `Schools`, `Subjects`, `Questions`, and `Tests` slices follow the intended direction. The `QuestionTypes`, `QuestionDifficulties`, and `TestTypes` features currently have read-only endpoints.

## Current Implementation Status

### Already implemented

- solution split into four projects
- domain entity model (11 entities)
- EF Core `DbContext`
- entity configurations for the full DB schema (11 configurations)
- MySQL provider setup in Infrastructure
- central package management
- Application DI registration through `AddApplication()`
- MediatR scanning of the Application assembly
- FluentValidation validator registration
- validation pipeline behavior in the Application layer
- `IApplicationDbContext` abstraction with `SaveChangesAsync`
- Infrastructure registration from the API startup path
- global exception middleware in the API layer
- full CQRS for `Classes` (get-all, get-by-id, create, update, delete)
- full CQRS for `Schools` (get-all, get-by-id, create, update, delete)
- full CQRS for `Subjects` (get-all, get-by-id, create, update, delete)
- full CQRS for `Questions` (get-all, get-by-id, create, update, delete)
- full CQRS for `Tests` (get-all, get-by-id, create, update, delete)
- read-only endpoints for `QuestionTypes`, `QuestionDifficulties`, `TestTypes` (get-all only)
- Contract separation pattern: `Contracts/` for DTOs, `Common/` for business logic

### Entity coverage matrix

| Entity | Domain | Config | GetAll | GetById | Create | Update | Delete |
|---|---|---|---|---|---|---|---|
| Class | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Subject | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| School | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| QuestionType | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| QuestionDifficulty | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| TestType | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| QuestionDetail | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Test | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| QuestionImage | ✅ | ✅ | — | — | ✅ | ✅ | ✅ |
| TestQuestionMap | ✅ | ✅ | — | — | ✅ | ✅ | ✅ |
| QuestionSubquestionMap | ✅ | ✅ | — | — | ✅ | ✅ | ✅ |

### Not implemented or still limited

- automated tests for handlers, validators, middleware, and endpoints
- a broader shared convention for translating `Result` values to HTTP responses across all endpoints

### Why this matters for contributors

New contributors should treat the current codebase as a working backend foundation with five complete vertical slices and three read-only features. The `Classes`, `Schools`, `Subjects`, `Questions`, and `Tests` features should be used as reference patterns for future slices. The Contract separation pattern (`Contracts/` for DTOs, `Common/` for business logic) is the standard approach for all features.

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
	},
	"Database": {
		"MySqlServerVersion": "8.0.36"
	}
}
```

Use local development credentials appropriate to your environment. Avoid committing real credentials into documentation or shared config. Set `Database:MySqlServerVersion` to the MySQL server version used by your environment; this avoids probing the database during application startup.

### Launch profiles

The API project includes two local launch profiles:

- HTTP: `http://localhost:5216`
- HTTPS: `https://localhost:7294`

## Development Notes

### Current startup behavior

`Program.cs` now registers controllers, Swagger, `AddApplication()`, `AddInfrastructure(builder.Configuration)`, and `ExceptionHandlingMiddleware`. This means the full request path from HTTP endpoint to handler to persistence is wired and runnable.

Swagger is available in the Development environment at `http://localhost:5216/swagger/index.html`. The raw OpenAPI document is available at `http://localhost:5216/swagger/v1/swagger.json`.

### Current error-handling convention

The repository currently uses this convention:

- expected business failures use `Result` / `Result<T>`
- validation failures are thrown from `ValidationBehavior`
- unexpected runtime failures are handled by API middleware

Contributors should follow that pattern unless the project intentionally decides to standardize on a different model later.

### Package management

NuGet package versions are managed centrally through `Directory.Packages.props`. Project files reference packages without repeating version numbers.

## Recommended Next Architecture Steps

1. Add automated tests for validators, handlers, middleware, and endpoints.
2. Standardize error handling with a typed `Result` that carries error codes for proper HTTP status mapping.
3. Keep the API layer limited to transport concerns, middleware, and endpoint orchestration as more features are added.

## Summary

This repository is currently a layered .NET backend with a working CQRS foundation, central validation, infrastructure wiring, exception middleware, five complete vertical slices (`Classes`, `Schools`, `Subjects`, `Questions`, `Tests`), and three read-only features (`QuestionTypes`, `QuestionDifficulties`, `TestTypes`). The Contract separation pattern (`Contracts/` for DTOs, `Common/` for business logic) is the standard approach for all features.