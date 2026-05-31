namespace testmaker.Application.Features.Questions.Contracts;

/// <summary>
/// Output DTO for a question image.
/// Returned when reading question details.
/// </summary>
public sealed record QuestionImageDto(Guid Id, string ImageName, int ImagePosition);
