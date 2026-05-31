namespace testmaker.Application.Features.Questions.Contracts;

/// <summary>
/// Input DTO for a question image.
/// Used when creating or updating questions with images.
/// </summary>
public sealed record QuestionImageRequest(string ImageName);
