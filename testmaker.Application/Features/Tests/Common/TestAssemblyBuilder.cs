using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Questions.Common;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Tests.Common;

internal static class TestAssemblyBuilder
{
    public static async Task<Result> PopulateTestAsync(
        Guid testId,
        Guid classId,
        Guid subjectId,
        IReadOnlyList<TestQuestionInput> questions,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        for (var questionIndex = 0; questionIndex < questions.Count; questionIndex++)
        {
            var questionInput = questions[questionIndex];
            var questionResult = await ResolveQuestionIdAsync(
                questionInput.ExistingQuestionId,
                questionInput.NewQuestion,
                classId,
                subjectId,
                context,
                cancellationToken);

            if (questionResult.IsFailure)
            {
                return questionResult;
            }

            var parentQuestionId = questionResult.Value;
            context.TestQuestionMaps.Add(new TestQuestionMap
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                QuestionId = parentQuestionId,
                QuestionPosition = questionIndex + 1
            });

            var subQuestions = questionInput.SubQuestions ?? [];
            for (var subquestionIndex = 0; subquestionIndex < subQuestions.Count; subquestionIndex++)
            {
                var subquestionInput = subQuestions[subquestionIndex];
                var subquestionResult = await ResolveQuestionIdAsync(
                    subquestionInput.ExistingQuestionId,
                    subquestionInput.NewQuestion,
                    classId,
                    subjectId,
                    context,
                    cancellationToken);

                if (subquestionResult.IsFailure)
                {
                    return subquestionResult;
                }

                if (subquestionResult.Value == parentQuestionId)
                {
                    return Result.Failure(
                        "A sub-question cannot reference the same question as its parent.",
                        ErrorType.Validation);
                }

                context.QuestionSubquestionMaps.Add(new QuestionSubquestionMap
                {
                    Id = Guid.NewGuid(),
                    TestId = testId,
                    QuestionId = parentQuestionId,
                    SubquestionId = subquestionResult.Value,
                    SubquestionNumber = subquestionIndex + 1
                });
            }
        }

        return Result.Success();
    }

    private static async Task<Result<Guid>> ResolveQuestionIdAsync(
        Guid? existingQuestionId,
        QuestionPayload? newQuestion,
        Guid classId,
        Guid subjectId,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var hasExistingQuestionId = existingQuestionId.HasValue;
        var hasNewQuestion = newQuestion is not null;

        if (hasExistingQuestionId == hasNewQuestion)
        {
            return Result<Guid>.Failure(
                "Each test question entry must specify exactly one of existingQuestionId or newQuestion.",
                ErrorType.Validation);
        }

        if (existingQuestionId.HasValue)
        {
            var existingQuestion = await context.QuestionDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.Id == existingQuestionId.Value, cancellationToken);

            if (existingQuestion is null)
            {
                return Result<Guid>.Failure(
                    $"Question '{existingQuestionId.Value}' was not found.",
                    ErrorType.Validation);
            }

            if (existingQuestion.ClassId != classId || existingQuestion.SubjectId != subjectId)
            {
                return Result<Guid>.Failure(
                    $"Question '{existingQuestionId.Value}' does not match the test class and subject.",
                    ErrorType.Validation);
            }

            return Result<Guid>.Success(existingQuestionId.Value);
        }

        if (newQuestion!.ClassId != classId || newQuestion.SubjectId != subjectId)
        {
            return Result<Guid>.Failure(
                "Inline questions must use the same classId and subjectId as the test.",
                ErrorType.Validation);
        }

        var referenceValidation = await QuestionContracts.ValidateReferencesAsync(
            newQuestion,
            context,
            cancellationToken);

        if (referenceValidation.IsFailure)
        {
            return Result<Guid>.Failure(referenceValidation.Error!, referenceValidation.ErrorType);
        }

        var entity = new QuestionDetail
        {
            Id = Guid.NewGuid()
        };

        var payloadResult = QuestionContracts.ApplyPayload(entity, newQuestion, referenceValidation.Value!);
        if (payloadResult.IsFailure)
        {
            return Result<Guid>.Failure(payloadResult.Error!, payloadResult.ErrorType);
        }

        context.QuestionDetails.Add(entity);

        var images = QuestionContracts.CreateImageEntities(entity.Id, newQuestion.Images);
        if (images.Count > 0)
        {
            context.QuestionImages.AddRange(images);
        }

        return Result<Guid>.Success(entity.Id);
    }
}