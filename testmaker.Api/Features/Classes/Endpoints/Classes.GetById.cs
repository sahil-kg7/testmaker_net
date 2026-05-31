using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;
using testmaker.Application.Features.Classes.Queries.GetClassById;

namespace testmaker.Api.Features.Classes.Endpoints;

public static class ClassesGetById
{
    public static RouteHandlerBuilder MapClassesGetById(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetClassByIdQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<ClassDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Classes")
        .WithName("GetClassById");
    }
}
