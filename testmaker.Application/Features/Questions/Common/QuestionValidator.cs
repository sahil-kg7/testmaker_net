using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Questions.Contracts;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Questions.Common;

/// <summary>
/// Handles validation and entity population for questions.
/// Contains business logic for applying question payloads to entities.
/// </summary>
internal static class QuestionValidator
{
    /// <summary>
    /// Validates that all referenced entities (question type, subject, class, difficulty) exist.
    /// Returns the question type name if validation passes.
    /// </summary>
    public static async Task<Result<string>> ValidateReferencesAsync(
        QuestionRequest request,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var questionType = await context.QuestionTypes
            .Where(entity => entity.Id == request.QuestionTypeId)
            .Select(entity => entity.Type)
            .FirstOrDefaultAsync(cancellationToken);

        if (questionType is null)
        {
            return Result<string>.Failure(
                $"Question type '{request.QuestionTypeId}' was not found.",
                ErrorType.Validation);
        }

        var subjectExists = await context.Subjects
            .AnyAsync(entity => entity.Id == request.SubjectId, cancellationToken);

        if (!subjectExists)
        {
            return Result<string>.Failure(
                $"Subject '{request.SubjectId}' was not found.",
                ErrorType.Validation);
        }

        var classExists = await context.Classes
            .AnyAsync(entity => entity.Id == request.ClassId, cancellationToken);

        if (!classExists)
        {
            return Result<string>.Failure(
                $"Class '{request.ClassId}' was not found.",
                ErrorType.Validation);
        }

        var difficultyExists = await context.QuestionDifficulties
            .AnyAsync(entity => entity.Id == request.Difficulty, cancellationToken);

        if (!difficultyExists)
        {
            return Result<string>.Failure(
                $"Question difficulty '{request.Difficulty}' was not found.",
                ErrorType.Validation);
        }

        return Result<string>.Success(questionType);
    }

    /// <summary>
    /// Applies the question request data to a QuestionDetail entity.
    /// Validates type-specific fields and normalizes text values.
    /// </summary>
    public static Result ApplyRequest(QuestionDetail entity, QuestionRequest request, string questionTypeName)
    {
        var questionTypeKind = ParseQuestionTypeKind(questionTypeName);
        var validationResult = ValidateTypeSpecificFields(request, questionTypeKind, questionTypeName);

        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        entity.QuestionTypeId = request.QuestionTypeId;
        entity.SubjectId = request.SubjectId;
        entity.ClassId = request.ClassId;
        entity.Difficulty = request.Difficulty;
        entity.Marks = request.Marks;
        entity.Content = NormalizeText(request.Content);
        entity.Mcq = null;
        entity.MatchA = null;
        entity.MatchB = null;
        entity.FibWords = null;
        entity.Reason = null;
        entity.Assertion = null;

        switch (questionTypeKind)
        {
            case QuestionTypeKind.Mcq:
                entity.Mcq = NormalizeItems(request.Mcq);
                break;
            case QuestionTypeKind.MatchTheFollowing:
                entity.MatchA = NormalizeItems(request.MatchA);
                entity.MatchB = NormalizeItems(request.MatchB);
                break;
            case QuestionTypeKind.FillInTheBlank:
                entity.FibWords = NormalizeItems(request.FibWords);
                break;
            case QuestionTypeKind.AssertionReason:
                entity.Assertion = NormalizeText(request.Assertion);
                entity.Reason = NormalizeText(request.Reason);
                break;
        }

        return Result.Success();
    }

    /// <summary>
    /// Creates QuestionImage entities from the request images.
    /// </summary>
    public static List<QuestionImage> CreateImageEntities(Guid questionId, IReadOnlyList<QuestionImageRequest>? images)
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

    private static Result ValidateTypeSpecificFields(
        QuestionRequest request,
        QuestionTypeKind questionTypeKind,
        string questionTypeName)
    {
        var hasImages = request.Images is { Count: > 0 };

        if (string.IsNullOrWhiteSpace(request.Content) && !hasImages)
        {
            return Result.Failure(
                "Question content is required unless at least one image is provided.",
                ErrorType.Validation);
        }

        var hasMcq = HasItems(request.Mcq);
        var hasMatchA = HasItems(request.MatchA);
        var hasMatchB = HasItems(request.MatchB);
        var hasFibWords = HasItems(request.FibWords);
        var hasReason = !string.IsNullOrWhiteSpace(request.Reason);
        var hasAssertion = !string.IsNullOrWhiteSpace(request.Assertion);

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

                if (NormalizeItems(request.MatchA)!.Count != NormalizeItems(request.MatchB)!.Count)
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
