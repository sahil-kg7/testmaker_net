using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Features.Tests.Models;
using testmaker.Application.Features.Tests.Common;
using testmaker.Application.Features.Tests.Commands.UpdateTest;

namespace testmaker.Api.Features.Tests.Endpoints;

public static class TestsUpdate
{
    public static RouteHandlerBuilder MapTestsUpdate(this IEndpointRouteBuilder app)
    {
        return app.MapPut("/{id:guid}", async (Guid id, UpsertTestRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateTestCommand(
                id,
                request.FileName,
                request.SchoolId,
                request.ClassId,
                request.SubjectId,
                request.TestTypeId,
                request.Sections,
                request.TimeDuration,
                request.MaximumMarks,
                request.Questions.Select(q => q.ToInput()).ToList());
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<TestDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Tests")
        .WithName("UpdateTest");
    }
}
