using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Features.Classes.Models;
using testmaker.Application.Features.Classes.Commands.UpdateClass;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;

namespace testmaker.Api.Features.Classes.Endpoints;

public static class ClassesUpdate
{
    public static RouteHandlerBuilder MapClassesUpdate(this IEndpointRouteBuilder app)
    {
        return app.MapPut("/{id:guid}", async (Guid id, UpdateClassRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateClassCommand(id, request.ClassName);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<ClassDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .WithTags("Classes")
        .WithName("UpdateClass");
    }
}
