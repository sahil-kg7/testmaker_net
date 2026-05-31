using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Common.Models;
using testmaker.Application.Features.Questions.Common;
using testmaker.Application.Features.Questions.Commands.UpdateQuestion;

namespace testmaker.Api.Features.Questions.Endpoints;

public static class QuestionsUpdate
{
    public static RouteHandlerBuilder MapQuestionsUpdate(this IEndpointRouteBuilder app)
    {
        return app.MapPut("/{id:guid}", async (Guid id, UpsertQuestionRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateQuestionCommand(id, request.ToPayload());
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<QuestionDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Questions")
        .WithName("UpdateQuestion");
    }
}
