using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Features.Tests.Models;
using testmaker.Application.Features.Tests.Common;
using testmaker.Application.Features.Tests.Commands.CreateTest;

namespace testmaker.Api.Features.Tests.Endpoints;

public static class TestsCreate
{
    public static RouteHandlerBuilder MapTestsCreate(this IEndpointRouteBuilder app)
    {
        return app.MapPost("/", async (UpsertTestRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateTestCommand(
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
                ? Results.CreatedAtRoute("GetTestById", new { id = result.Value!.Id }, result.Value)
                : ErrorResult.From(result);
        })
        .Produces<TestDetailDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Tests")
        .WithName("CreateTest");
    }
}
