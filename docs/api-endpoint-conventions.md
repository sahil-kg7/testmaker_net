# API Endpoint Conventions

## Structure

Every HTTP endpoint is a single file under `testmaker.Api/Features/{Feature}/Endpoints/`.

```
testmaker.Api/Features/{Feature}/
├── Endpoints/
│   ├── {Feature}.{Endpoint}.cs     # one file per HTTP verb
│   └── ...
├── Models/
│   └── ...                          # request/response DTOs for this feature
└── {Feature}Endpoints.cs            # grouping file that registers all endpoints
```

## Naming

| Element | Convention | Example |
|---|---|---|
| **File name** | `{Feature}.{Endpoint}.cs` | `Classes.GetAll.cs` |
| **Class name** | `{Feature}{Endpoint}` | `ClassesGetAll` |
| **Extension method** | `Map{Feature}{Endpoint}(this IEndpointRouteBuilder)` | `MapClassesGetAll` |
| **Grouping method** | `Map{Feature}Endpoints(this IEndpointRouteBuilder)` | `MapClassEndpoints` |

Endpoint names: `GetAll`, `GetById`, `Create`, `Update`, `Delete`.

## When to create a new endpoint file

One file per HTTP verb per resource. Not one file per feature.

- `Schools.GetAll.cs` — `GET /api/schools`
- `Schools.GetById.cs` — `GET /api/schools/{id:guid}`
- `Schools.Create.cs` — `POST /api/schools`
- `Schools.Update.cs` — `PUT /api/schools/{id:guid}`
- `Schools.Delete.cs` — `DELETE /api/schools/{id:guid}`

Do NOT combine multiple HTTP verbs into a single file.

## Endpoint file template

```csharp
using MediatR;
using testmaker.Api.Common;
// Application-layer imports for the relevant query/command and response DTOs

namespace testmaker.Api.Features.{Feature}.Endpoints;

public static class {Feature}{Endpoint}
{
    public static RouteHandlerBuilder Map{Feature}{Endpoint}(this IEndpointRouteBuilder app)
    {
        return app.Map{Verb}("/", async (/* params */ ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SomeQuery(/* params */), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<ResponseDto>(StatusCodes.Status200OK)
        .WithTags("{Feature}")
        .WithName("{Feature}{Endpoint}");
    }
}
```

## Grouping file template

```csharp
using testmaker.Api.Features.{Feature}.Endpoints;

namespace testmaker.Api.Features.{Feature};

public static class {Feature}Endpoints
{
    public static IEndpointRouteBuilder Map{Feature}Endpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/{feature-route}");
        group.Map{Feature}GetAll();
        group.Map{Feature}GetById();
        group.Map{Feature}Create();
        group.Map{Feature}Update();
        group.Map{Feature}Delete();
        return app;
    }
}
```

## Registration in Program.cs

```csharp
app.Map{Feature}Endpoints();
```

## Error handling

Use the shared `ErrorResult.From(Result)` helper from `testmaker.Api.Common` instead of duplication.

```csharp
return result.IsSuccess
    ? Results.Ok(result.Value)
    : ErrorResult.From(result);
```

## Request DTO placement

- **Feature-specific request DTOs** → `Features/{Feature}/Models/*.cs`
- **Cross-feature shared request DTOs** → `Common/Models/*.cs`
- **Response DTOs** → reference from `testmaker.Application.Features.{Feature}` unless API-layer wrapping is needed

## Route templates

Use explicit route strings with `MapGroup`, not `[controller]` token replacement:

```csharp
var group = app.MapGroup("/api/classes");     // not "api/[controller]"
```

Use `:guid` constraint on ID parameters:

```csharp
app.MapGet("/{id:guid}", ...)
```
