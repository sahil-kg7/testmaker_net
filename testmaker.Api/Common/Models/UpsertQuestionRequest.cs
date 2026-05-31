using testmaker.Application.Features.Questions.Contracts;

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
    public QuestionRequest ToRequest()
    {
        return new QuestionRequest(
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
            Images?.Select(image => new Application.Features.Questions.Contracts.QuestionImageRequest(image.ImageName)).ToList());
    }
}
