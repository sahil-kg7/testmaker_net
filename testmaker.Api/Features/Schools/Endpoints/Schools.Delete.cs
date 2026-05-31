using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Schools.Commands.DeleteSchool;

namespace testmaker.Api.Features.Schools.Endpoints;

public static class SchoolsDelete
{
    public static RouteHandlerBuilder MapSchoolsDelete(this IEndpointRouteBuilder app)
    {
        return app.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteSchoolCommand(id), ct);
            return result.IsSuccess
                ? Results.Ok()
                : ErrorResult.From(result);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Schools")
        .WithName("DeleteSchool");
    }
}
