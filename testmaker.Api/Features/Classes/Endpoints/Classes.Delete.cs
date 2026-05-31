using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Classes.Commands.DeleteClass;

namespace testmaker.Api.Features.Classes.Endpoints;

public static class ClassesDelete
{
    public static RouteHandlerBuilder MapClassesDelete(this IEndpointRouteBuilder app)
    {
        return app.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteClassCommand(id), ct);
            return result.IsSuccess
                ? Results.Ok()
                : ErrorResult.From(result);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Classes")
        .WithName("DeleteClass");
    }
}
