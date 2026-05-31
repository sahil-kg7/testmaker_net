using testmaker.Application.Features.Questions.Common;

namespace testmaker.Api.Common.Models;

public sealed record UpsertQuestionRequest(
    Guid QuestionTypeId,
    Guid SubjectId,
    Guid ClassId,
    Guid Difficulty,
    int Marks,
    string? Content,
    IReadOnlyList<string>? Mcq,
    IReadOnlyList<string>? MatchA,
    IReadOnlyList<string>? MatchB,
    IReadOnlyList<string>? FibWords,
    string? Reason,
    string? Assertion,
    IReadOnlyList<QuestionImageRequest>? Images)
{
    public QuestionPayload ToPayload()
    {
        return new QuestionPayload(
            QuestionTypeId,
            SubjectId,
            ClassId,
            Difficulty,
            Marks,
            Content,
            Mcq,
            MatchA,
            MatchB,
            FibWords,
            Reason,
            Assertion,
            Images?.Select(image => new QuestionImageInput(image.ImageName)).ToList());
    }
}
