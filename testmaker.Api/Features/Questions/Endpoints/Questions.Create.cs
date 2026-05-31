using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Common.Models;
using testmaker.Application.Features.Questions.Common;
using testmaker.Application.Features.Questions.Commands.CreateQuestion;

namespace testmaker.Api.Features.Questions.Endpoints;

public static class QuestionsCreate
{
    public static RouteHandlerBuilder MapQuestionsCreate(this IEndpointRouteBuilder app)
    {
        return app.MapPost("/", async (UpsertQuestionRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateQuestionCommand(request.ToPayload());
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.CreatedAtRoute("GetQuestionById", new { id = result.Value!.Id }, result.Value)
                : ErrorResult.From(result);
        })
        .Produces<QuestionDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Questions")
        .WithName("CreateQuestion");
    }
}
