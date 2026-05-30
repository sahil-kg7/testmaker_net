using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Questions.Common;

public sealed record QuestionImageInput(string ImageName);

public sealed record QuestionPayload(
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
    IReadOnlyList<QuestionImageInput>? Images);

public sealed record QuestionImageDto(Guid Id, string ImageName, int ImagePosition);

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

internal enum QuestionTypeKind
{
    Generic,
    Mcq,
    MatchTheFollowing,
    FillInTheBlank,
    AssertionReason
}

internal static class QuestionContracts
{
    public static IQueryable<QuestionDetail> BuildDetailQuery(IApplicationDbContext context)
    {
        return context.QuestionDetails
            .AsNoTracking()
            .Include(question => question.QuestionType)
            .Include(question => question.Subject)
            .Include(question => question.Class)
            .Include(question => question.DifficultyNavigation)
            .Include(question => question.QuestionImages);
    }

    public static async Task<Result<string>> ValidateReferencesAsync(
        QuestionPayload payload,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var questionType = await context.QuestionTypes
            .Where(entity => entity.Id == payload.QuestionTypeId)
            .Select(entity => entity.Type)
            .FirstOrDefaultAsync(cancellationToken);

        if (questionType is null)
        {
            return Result<string>.Failure(
                $"Question type '{payload.QuestionTypeId}' was not found.",
                ErrorType.Validation);
        }

        var subjectExists = await context.Subjects
            .AnyAsync(entity => entity.Id == payload.SubjectId, cancellationToken);

        if (!subjectExists)
        {
            return Result<string>.Failure(
                $"Subject '{payload.SubjectId}' was not found.",
                ErrorType.Validation);
        }

        var classExists = await context.Classes
            .AnyAsync(entity => entity.Id == payload.ClassId, cancellationToken);

        if (!classExists)
        {
            return Result<string>.Failure(
                $"Class '{payload.ClassId}' was not found.",
                ErrorType.Validation);
        }

        var difficultyExists = await context.QuestionDifficulties
            .AnyAsync(entity => entity.Id == payload.Difficulty, cancellationToken);

        if (!difficultyExists)
        {
            return Result<string>.Failure(
                $"Question difficulty '{payload.Difficulty}' was not found.",
                ErrorType.Validation);
        }

        return Result<string>.Success(questionType);
    }

    public static Result ApplyPayload(QuestionDetail entity, QuestionPayload payload, string questionTypeName)
    {
        var questionTypeKind = ParseQuestionTypeKind(questionTypeName);
        var validationResult = ValidateTypeSpecificFields(payload, questionTypeKind, questionTypeName);

        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        entity.QuestionTypeId = payload.QuestionTypeId;
        entity.SubjectId = payload.SubjectId;
        entity.ClassId = payload.ClassId;
        entity.Difficulty = payload.Difficulty;
        entity.Marks = payload.Marks;
        entity.Content = NormalizeText(payload.Content);
        entity.Mcq = null;
        entity.MatchA = null;
        entity.MatchB = null;
        entity.FibWords = null;
        entity.Reason = null;
        entity.Assertion = null;

        switch (questionTypeKind)
        {
            case QuestionTypeKind.Mcq:
                entity.Mcq = NormalizeItems(payload.Mcq);
                break;
            case QuestionTypeKind.MatchTheFollowing:
                entity.MatchA = NormalizeItems(payload.MatchA);
                entity.MatchB = NormalizeItems(payload.MatchB);
                break;
            case QuestionTypeKind.FillInTheBlank:
                entity.FibWords = NormalizeItems(payload.FibWords);
                break;
            case QuestionTypeKind.AssertionReason:
                entity.Assertion = NormalizeText(payload.Assertion);
                entity.Reason = NormalizeText(payload.Reason);
                break;
        }

        return Result.Success();
    }

    public static List<QuestionImage> CreateImageEntities(Guid questionId, IReadOnlyList<QuestionImageInput>? images)
    {
        if (images is null || images.Count == 0)
        {
            return [];
        }

        return images
            .Select((image, index) => new QuestionImage
            {
                Id = Guid.NewGuid(),
                QuestionId = questionId,
                ImageName = image.ImageName.Trim(),
                ImagePosition = index + 1
            })
            .ToList();
    }

    public static QuestionDto ToQuestionDto(QuestionDetail entity)
    {
        var orderedImages = entity.QuestionImages
            .OrderBy(image => image.ImagePosition)
            .Select(image => new QuestionImageDto(image.Id, image.ImageName, image.ImagePosition))
            .ToList();

        return new QuestionDto(
            entity.Id,
            entity.QuestionTypeId,
            entity.QuestionType.Type,
            entity.SubjectId,
            entity.Subject.Name,
            entity.ClassId,
            entity.Class.ClassName,
            entity.Difficulty,
            entity.DifficultyNavigation.Level,
            entity.Marks,
            entity.Content,
            entity.Mcq,
            entity.MatchA,
            entity.MatchB,
            entity.FibWords,
            entity.Reason,
            entity.Assertion,
            orderedImages,
            entity.CreatedOn,
            entity.UpdatedOn);
    }

    public static QuestionListItemDto ToQuestionListItemDto(QuestionDetail entity)
    {
        return new QuestionListItemDto(
            entity.Id,
            entity.QuestionTypeId,
            entity.QuestionType.Type,
            entity.SubjectId,
            entity.Subject.Name,
            entity.ClassId,
            entity.Class.ClassName,
            entity.Difficulty,
            entity.DifficultyNavigation.Level,
            entity.Marks,
            BuildContentPreview(entity.Content),
            entity.QuestionImages.Count > 0,
            entity.CreatedOn,
            entity.UpdatedOn);
    }

    public static string? BuildContentPreview(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        const int maxLength = 140;

        var normalized = content.Trim();
        if (normalized.Length <= maxLength)
        {
            return normalized;
        }

        return $"{normalized[..(maxLength - 3)]}...";
    }

    private static Result ValidateTypeSpecificFields(
        QuestionPayload payload,
        QuestionTypeKind questionTypeKind,
        string questionTypeName)
    {
        var hasImages = payload.Images is { Count: > 0 };

        if (string.IsNullOrWhiteSpace(payload.Content) && !hasImages)
        {
            return Result.Failure(
                "Question content is required unless at least one image is provided.",
                ErrorType.Validation);
        }

        var hasMcq = HasItems(payload.Mcq);
        var hasMatchA = HasItems(payload.MatchA);
        var hasMatchB = HasItems(payload.MatchB);
        var hasFibWords = HasItems(payload.FibWords);
        var hasReason = !string.IsNullOrWhiteSpace(payload.Reason);
        var hasAssertion = !string.IsNullOrWhiteSpace(payload.Assertion);

        switch (questionTypeKind)
        {
            case QuestionTypeKind.Mcq:
                if (!hasMcq)
                {
                    return Result.Failure("MCQ questions require at least one option.", ErrorType.Validation);
                }

                if (hasMatchA || hasMatchB || hasFibWords || hasReason || hasAssertion)
                {
                    return Result.Failure(
                        $"Question type '{questionTypeName}' only supports the mcq field.",
                        ErrorType.Validation);
                }

                break;
            case QuestionTypeKind.MatchTheFollowing:
                if (!hasMatchA || !hasMatchB)
                {
                    return Result.Failure(
                        "Match the Following questions require both matchA and matchB values.",
                        ErrorType.Validation);
                }

                if (NormalizeItems(payload.MatchA)!.Count != NormalizeItems(payload.MatchB)!.Count)
                {
                    return Result.Failure(
                        "Match the Following questions require matchA and matchB to have the same number of entries.",
                        ErrorType.Validation);
                }

                if (hasMcq || hasFibWords || hasReason || hasAssertion)
                {
                    return Result.Failure(
                        $"Question type '{questionTypeName}' only supports the matchA and matchB fields.",
                        ErrorType.Validation);
                }

                break;
            case QuestionTypeKind.FillInTheBlank:
                if (!hasFibWords)
                {
                    return Result.Failure(
                        "Fill in the Blank questions require fibWords values.",
                        ErrorType.Validation);
                }

                if (hasMcq || hasMatchA || hasMatchB || hasReason || hasAssertion)
                {
                    return Result.Failure(
                        $"Question type '{questionTypeName}' only supports the fibWords field.",
                        ErrorType.Validation);
                }

                break;
            case QuestionTypeKind.AssertionReason:
                if (!hasAssertion || !hasReason)
                {
                    return Result.Failure(
                        "Assertion-Reason questions require both assertion and reason.",
                        ErrorType.Validation);
                }

                if (hasMcq || hasMatchA || hasMatchB || hasFibWords)
                {
                    return Result.Failure(
                        $"Question type '{questionTypeName}' only supports the assertion and reason fields.",
                        ErrorType.Validation);
                }

                break;
            default:
                if (hasMcq || hasMatchA || hasMatchB || hasFibWords || hasReason || hasAssertion)
                {
                    return Result.Failure(
                        $"Question type '{questionTypeName}' does not support conditional question payload fields.",
                        ErrorType.Validation);
                }

                break;
        }

        return Result.Success();
    }

    private static QuestionTypeKind ParseQuestionTypeKind(string questionTypeName)
    {
        var normalized = new string(questionTypeName
            .Where(char.IsLetterOrDigit)
            .ToArray())
            .ToLowerInvariant();

        return normalized switch
        {
            "mcq" => QuestionTypeKind.Mcq,
            "matchthefollowing" => QuestionTypeKind.MatchTheFollowing,
            "fillintheblank" => QuestionTypeKind.FillInTheBlank,
            "fib" => QuestionTypeKind.FillInTheBlank,
            "assertionreason" => QuestionTypeKind.AssertionReason,
            _ => QuestionTypeKind.Generic
        };
    }

    private static bool HasItems(IReadOnlyList<string>? values)
    {
        return values is not null && values.Any(value => !string.IsNullOrWhiteSpace(value));
    }

    private static List<string>? NormalizeItems(IReadOnlyList<string>? values)
    {
        if (values is null)
        {
            return null;
        }

        var normalized = values
            .Select(value => value?.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Cast<string>()
            .ToList();

        return normalized.Count == 0 ? null : normalized;
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}