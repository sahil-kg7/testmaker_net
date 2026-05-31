using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Questions.Common;
using testmaker.Application.Features.Questions.Queries.GetQuestionById;

namespace testmaker.Api.Features.Questions.Endpoints;

public static class QuestionsGetById
{
    public static RouteHandlerBuilder MapQuestionsGetById(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetQuestionByIdQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<QuestionDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Questions")
        .WithName("GetQuestionById");
    }
}
