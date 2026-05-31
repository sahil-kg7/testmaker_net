namespace testmaker.Application.Features.Questions.Contracts;

/// <summary>
/// Output DTO for a full question detail.
/// Contains all question data including related names and images.
/// </summary>
public sealed record QuestionDto(
    Guid Id,
    Guid QuestionTypeId,
    string QuestionType,
    Guid SubjectId,
    string SubjectName,
    Guid ClassId,
    string ClassName,
    Guid Difficulty,
    string DifficultyLevel,
    int Marks,
    string? Content,
    IReadOnlyList<string>? Mcq,
    IReadOnlyList<string>? MatchA,
    IReadOnlyList<string>? MatchB,
    IReadOnlyList<string>? FibWords,
    string? Reason,
    string? Assertion,
    IReadOnlyList<QuestionImageDto> Images,
    DateTime CreatedOn,
    DateTime UpdatedOn);
