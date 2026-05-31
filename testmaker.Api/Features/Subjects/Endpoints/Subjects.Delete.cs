using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Subjects.Commands.DeleteSubject;

namespace testmaker.Api.Features.Subjects.Endpoints;

public static class SubjectsDelete
{
    public static RouteHandlerBuilder MapSubjectsDelete(this IEndpointRouteBuilder app)
    {
        return app.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteSubjectCommand(id), ct);
            return result.IsSuccess
                ? Results.Ok()
                : ErrorResult.From(result);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Subjects")
        .WithName("DeleteSubject");
    }
}
