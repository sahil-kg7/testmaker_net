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
        var questionTypeKind = ParseQuestionTypeKindEnum(questionTypeName);
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
            case QuestionType.Mcq:
                entity.Mcq = NormalizeItems(request.Mcq);
                break;
            case QuestionType.MatchTheFollowing:
                entity.MatchA = NormalizeItems(request.MatchA);
                entity.MatchB = NormalizeItems(request.MatchB);
                break;
            case QuestionType.FillInTheBlank:
                entity.FibWords = NormalizeItems(request.FibWords);
                break;
            case QuestionType.AssertionReason:
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

    private readonly record struct QuestionFieldState(
        bool Mcq,
        bool MatchA,
        bool MatchB,
        bool FibWords,
        bool Reason,
        bool Assertion);

    [Flags]
    private enum QuestionFields
    {
        None = 0,
        Mcq = 1,
        MatchA = 2,
        MatchB = 4,
        FibWords = 8,
        Reason = 16,
        Assertion = 32,
    }

    private sealed record TypeRules(
        QuestionFields RequiredFields,
        string RequiredFieldsError,
        QuestionFields ForbiddenFields);

    private static readonly Dictionary<QuestionType, TypeRules> TypeRulesMap = new()
    {
        [QuestionType.Mcq] = new TypeRules(
            RequiredFields: QuestionFields.Mcq,
            RequiredFieldsError: "MCQ questions require at least one option.",
            ForbiddenFields: QuestionFields.MatchA | QuestionFields.MatchB | QuestionFields.FibWords | QuestionFields.Reason | QuestionFields.Assertion),
        [QuestionType.MatchTheFollowing] = new TypeRules(
            RequiredFields: QuestionFields.MatchA | QuestionFields.MatchB,
            RequiredFieldsError: "Match the Following questions require both matchA and matchB values.",
            ForbiddenFields: QuestionFields.Mcq | QuestionFields.FibWords | QuestionFields.Reason | QuestionFields.Assertion),
        [QuestionType.FillInTheBlank] = new TypeRules(
            RequiredFields: QuestionFields.FibWords,
            RequiredFieldsError: "Fill in the Blank questions require fibWords values.",
            ForbiddenFields: QuestionFields.Mcq | QuestionFields.MatchA | QuestionFields.MatchB | QuestionFields.Reason | QuestionFields.Assertion),
        [QuestionType.AssertionReason] = new TypeRules(
            RequiredFields: QuestionFields.Reason | QuestionFields.Assertion,
            RequiredFieldsError: "Assertion-Reason questions require both assertion and reason.",
            ForbiddenFields: QuestionFields.Mcq | QuestionFields.MatchA | QuestionFields.MatchB | QuestionFields.FibWords),
        [QuestionType.Generic] = new TypeRules(
            RequiredFields: QuestionFields.None,
            RequiredFieldsError: "",
            ForbiddenFields: QuestionFields.Mcq | QuestionFields.MatchA | QuestionFields.MatchB | QuestionFields.FibWords | QuestionFields.Reason | QuestionFields.Assertion),
    };

    private static Result ValidateTypeSpecificFields(
        QuestionRequest request,
        QuestionType questionTypeKind,
        string questionTypeName)
    {
        var hasImages = request.Images is { Count: > 0 };

        if (string.IsNullOrWhiteSpace(request.Content) && !hasImages)
        {
            return Result.Failure(
                "Question content is required unless at least one image is provided.",
                ErrorType.Validation);
        }

        var state = new QuestionFieldState(
            Mcq: HasItems(request.Mcq),
            MatchA: HasItems(request.MatchA),
            MatchB: HasItems(request.MatchB),
            FibWords: HasItems(request.FibWords),
            Reason: !string.IsNullOrWhiteSpace(request.Reason),
            Assertion: !string.IsNullOrWhiteSpace(request.Assertion));

        var rules = TypeRulesMap[questionTypeKind];
        var presentFields = ToFieldFlags(state);

        if ((presentFields & rules.RequiredFields) != rules.RequiredFields)
        {
            return Result.Failure(rules.RequiredFieldsError, ErrorType.Validation);
        }

        if ((presentFields & rules.ForbiddenFields) != 0)
        {
            return Result.Failure(
                $"Question type '{questionTypeName}' only supports its designated fields.",
                ErrorType.Validation);
        }

        if (questionTypeKind == QuestionType.MatchTheFollowing
            && NormalizeItems(request.MatchA)!.Count != NormalizeItems(request.MatchB)!.Count)
        {
            return Result.Failure(
                "Match the Following questions require matchA and matchB to have the same number of entries.",
                ErrorType.Validation);
        }

        return Result.Success();
    }

    private static QuestionFields ToFieldFlags(QuestionFieldState state)
    {
        var flags = QuestionFields.None;
        if (state.Mcq) flags |= QuestionFields.Mcq;
        if (state.MatchA) flags |= QuestionFields.MatchA;
        if (state.MatchB) flags |= QuestionFields.MatchB;
        if (state.FibWords) flags |= QuestionFields.FibWords;
        if (state.Reason) flags |= QuestionFields.Reason;
        if (state.Assertion) flags |= QuestionFields.Assertion;
        return flags;
    }

    private static QuestionType ParseQuestionTypeKindEnum(string questionTypeName)
    {
        var normalized = new string(questionTypeName
            .Where(char.IsLetterOrDigit)
            .ToArray())
            .ToLowerInvariant();

        return normalized switch
        {
            "mcq" => QuestionType.Mcq,
            "matchthefollowing" => QuestionType.MatchTheFollowing,
            "fillintheblank" => QuestionType.FillInTheBlank,
            "fib" => QuestionType.FillInTheBlank,
            "assertionreason" => QuestionType.AssertionReason,
            _ => QuestionType.Generic
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
