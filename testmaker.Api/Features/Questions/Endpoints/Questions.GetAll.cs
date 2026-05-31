using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Questions.Contracts;
using testmaker.Application.Features.Questions.Queries.GetQuestions;

namespace testmaker.Api.Features.Questions.Endpoints;

public static class QuestionsGetAll
{
    public static RouteHandlerBuilder MapQuestionsGetAll(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", async (
            Guid? classId,
            Guid? subjectId,
            Guid? questionTypeId,
            Guid? difficultyId,
            string? search,
            int? page,
            int? pageSize,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetQuestionsQuery(classId, subjectId, questionTypeId, difficultyId, search, page, pageSize);
            var result = await sender.Send(query, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<List<QuestionListItemDto>>(StatusCodes.Status200OK)
        .WithTags("Questions")
        .WithName("GetAllQuestions");
    }
}
