using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;

namespace testmaker.Api.Features.Subjects.Endpoints;

public static class SubjectsGetAll
{
    public static RouteHandlerBuilder MapSubjectsGetAll(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllSubjectsQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<List<SubjectDto>>(StatusCodes.Status200OK)
        .WithTags("Subjects")
        .WithName("GetAllSubjects");
    }
}
