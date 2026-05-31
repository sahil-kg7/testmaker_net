using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Features.Classes.Models;
using testmaker.Application.Features.Classes.Commands.CreateClass;

namespace testmaker.Api.Features.Classes.Endpoints;

public static class ClassesCreate
{
    public static RouteHandlerBuilder MapClassesCreate(this IEndpointRouteBuilder app)
    {
        return app.MapPost("/", async (CreateClassRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateClassCommand(request.ClassName);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.CreatedAtRoute("GetClassById", new { id = result.Value }, result.Value)
                : ErrorResult.From(result);
        })
        .Produces<Guid>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict)
        .WithTags("Classes")
        .WithName("CreateClass");
    }
}
