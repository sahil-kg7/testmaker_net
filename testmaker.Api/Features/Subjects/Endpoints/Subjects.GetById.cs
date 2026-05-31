using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;
using testmaker.Application.Features.Subjects.Queries.GetSubjectById;

namespace testmaker.Api.Features.Subjects.Endpoints;

public static class SubjectsGetById
{
    public static RouteHandlerBuilder MapSubjectsGetById(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSubjectByIdQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<SubjectDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Subjects")
        .WithName("GetSubjectById");
    }
}
