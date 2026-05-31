namespace testmaker.Application.Features.Questions.Contracts;

/// <summary>
/// Input DTO for creating or updating a question.
/// Contains all fields needed to define a question regardless of its type.
/// </summary>
public sealed record QuestionRequest(
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
    IReadOnlyList<QuestionImageRequest>? Images);
