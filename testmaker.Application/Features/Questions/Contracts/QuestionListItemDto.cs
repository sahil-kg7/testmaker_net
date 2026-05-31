namespace testmaker.Application.Features.Questions.Contracts;

/// <summary>
/// Output DTO for a question in list view.
/// Contains a preview of the question data for list endpoints.
/// </summary>
public sealed record QuestionListItemDto(
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
    string? ContentPreview,
    bool HasImages,
    DateTime CreatedOn,
    DateTime UpdatedOn);
