using testmaker.Api.Common.Models;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Api.Features.Questions.Models;

internal static class QuestionMapping
{
    public static QuestionPayload ToPayload(this UpsertQuestionRequest request)
        => request.ToPayload();
}
