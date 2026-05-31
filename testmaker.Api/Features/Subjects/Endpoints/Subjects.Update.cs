using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Features.Subjects.Models;
using testmaker.Application.Features.Subjects.Commands.UpdateSubject;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;

namespace testmaker.Api.Features.Subjects.Endpoints;

public static class SubjectsUpdate
{
    public static RouteHandlerBuilder MapSubjectsUpdate(this IEndpointRouteBuilder app)
    {
        return app.MapPut("/{id:guid}", async (Guid id, UpdateSubjectRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateSubjectCommand(id, request.Name);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<SubjectDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .WithTags("Subjects")
        .WithName("UpdateSubject");
    }
}
