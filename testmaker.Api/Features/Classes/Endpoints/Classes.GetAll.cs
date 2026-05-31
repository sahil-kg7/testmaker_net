using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;

namespace testmaker.Api.Features.Classes.Endpoints;

public static class ClassesGetAll
{
    public static RouteHandlerBuilder MapClassesGetAll(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllClassesQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<List<ClassDto>>(StatusCodes.Status200OK)
        .WithTags("Classes")
        .WithName("GetAllClasses");
    }
}
