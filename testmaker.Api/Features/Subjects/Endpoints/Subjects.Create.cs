using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Features.Subjects.Models;
using testmaker.Application.Features.Subjects.Commands.CreateSubject;

namespace testmaker.Api.Features.Subjects.Endpoints;

public static class SubjectsCreate
{
    public static RouteHandlerBuilder MapSubjectsCreate(this IEndpointRouteBuilder app)
    {
        return app.MapPost("/", async (CreateSubjectRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateSubjectCommand(request.Name);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.CreatedAtRoute("GetSubjectById", new { id = result.Value!.Id }, result.Value)
                : ErrorResult.From(result);
        })
        .Produces<CreateSubjectResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict)
        .WithTags("Subjects")
        .WithName("CreateSubject");
    }
}
