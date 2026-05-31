using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Questions.Common;
using testmaker.Application.Features.Questions.Contracts;

namespace testmaker.Application.Features.Tests.Common;

public sealed record TestSubquestionInput(Guid? ExistingQuestionId, QuestionRequest? NewQuestion);

public sealed record TestQuestionInput(
    Guid? ExistingQuestionId,
    QuestionRequest? NewQuestion,
    IReadOnlyList<TestSubquestionInput>? SubQuestions);

public sealed record TestListItemDto(
    Guid Id,
    string FileName,
    Guid SchoolId,
    string SchoolName,
    Guid ClassId,
    string ClassName,
    Guid SubjectId,
    string SubjectName,
    Guid TestTypeId,
    string TestType,
    int TimeDuration,
    int MaximumMarks,
    int QuestionCount,
    DateTime CreatedOn,
    DateTime UpdatedOn);

public sealed record TestSubquestionBriefDto(
    Guid QuestionId,
    int SubquestionNumber,
    string? ContentPreview,
    int Marks);

public sealed record TestQuestionBriefDto(
    Guid QuestionId,
    int QuestionPosition,
    string QuestionType,
    string DifficultyLevel,
    int Marks,
    string? ContentPreview,
    bool HasImages,
    IReadOnlyList<TestSubquestionBriefDto> SubQuestions);

public sealed record TestDetailDto(
    Guid Id,
    string FileName,
    Guid SchoolId,
    string SchoolName,
    Guid ClassId,
    string ClassName,
    Guid SubjectId,
    string SubjectName,
    Guid TestTypeId,
    string TestType,
    IReadOnlyList<int>? Sections,
    int TimeDuration,
    int MaximumMarks,
    IReadOnlyList<TestQuestionBriefDto> Questions,
    DateTime CreatedOn,
    DateTime UpdatedOn);

internal static class TestContracts
{
    public static async Task<Result> ValidateReferencesAsync(
        Guid schoolId,
        Guid classId,
        Guid subjectId,
        Guid testTypeId,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var schoolExists = await context.Schools.AnyAsync(entity => entity.Id == schoolId, cancellationToken);
        if (!schoolExists)
        {
            return Result.Failure($"School '{schoolId}' was not found.", ErrorType.Validation);
        }

        var classExists = await context.Classes.AnyAsync(entity => entity.Id == classId, cancellationToken);
        if (!classExists)
        {
            return Result.Failure($"Class '{classId}' was not found.", ErrorType.Validation);
        }

        var subjectExists = await context.Subjects.AnyAsync(entity => entity.Id == subjectId, cancellationToken);
        if (!subjectExists)
        {
            return Result.Failure($"Subject '{subjectId}' was not found.", ErrorType.Validation);
        }

        var testTypeExists = await context.TestTypes.AnyAsync(entity => entity.Id == testTypeId, cancellationToken);
        if (!testTypeExists)
        {
            return Result.Failure($"Test type '{testTypeId}' was not found.", ErrorType.Validation);
        }

        return Result.Success();
    }

    public static Result ValidateSections(IReadOnlyList<int>? sections, int questionCount)
    {
        if (sections is null || sections.Count == 0)
        {
            return Result.Success();
        }

        if (sections.Any(section => section <= 0 || section > questionCount))
        {
            return Result.Failure(
                "Sections must contain unique question positions within the test question count.",
                ErrorType.Validation);
        }

        if (sections.Count != sections.Distinct().Count())
        {
            return Result.Failure("Sections must not contain duplicate question positions.", ErrorType.Validation);
        }

        return Result.Success();
    }

    public static async Task<TestDetailDto?> LoadTestDetailAsync(
        IApplicationDbContext context,
        Guid testId,
        CancellationToken cancellationToken)
    {
        var test = await context.Tests
            .AsNoTracking()
            .Include(entity => entity.School)
            .Include(entity => entity.Class)
            .Include(entity => entity.Subject)
            .Include(entity => entity.TestType)
            .FirstOrDefaultAsync(entity => entity.Id == testId, cancellationToken);

        if (test is null)
        {
            return null;
        }

        var questionMaps = await context.TestQuestionMaps
            .AsNoTracking()
            .Where(entity => entity.TestId == testId)
            .OrderBy(entity => entity.QuestionPosition)
            .ToListAsync(cancellationToken);

        var subquestionMaps = await context.QuestionSubquestionMaps
            .AsNoTracking()
            .Where(entity => entity.TestId == testId)
            .OrderBy(entity => entity.QuestionId)
            .ThenBy(entity => entity.SubquestionNumber)
            .ToListAsync(cancellationToken);

        var questionIds = questionMaps.Select(entity => entity.QuestionId)
            .Concat(subquestionMaps.Select(entity => entity.SubquestionId))
            .Distinct()
            .ToList();

        var questionLookup = questionIds.Count == 0
            ? new Dictionary<Guid, Domain.Entities.QuestionDetail>()
            : await QuestionMapper.BuildDetailQuery(context)
                .Where(entity => questionIds.Contains(entity.Id))
                .ToDictionaryAsync(entity => entity.Id, cancellationToken);

        var subquestionsByParentId = subquestionMaps
            .GroupBy(entity => entity.QuestionId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Where(entity => questionLookup.ContainsKey(entity.SubquestionId))
                    .Select(entity =>
                    {
                        var question = questionLookup[entity.SubquestionId];
                        return new TestSubquestionBriefDto(
                            entity.SubquestionId,
                            entity.SubquestionNumber,
                            QuestionMapper.BuildContentPreview(question.Content),
                            question.Marks);
                    })
                    .ToList()
                    .AsReadOnly());

        var questions = questionMaps
            .Where(entity => questionLookup.ContainsKey(entity.QuestionId))
            .Select(entity =>
            {
                var question = questionLookup[entity.QuestionId];
                IReadOnlyList<TestSubquestionBriefDto> subquestions =
                    subquestionsByParentId.TryGetValue(entity.QuestionId, out var mappedSubquestions)
                        ? mappedSubquestions
                        : Array.Empty<TestSubquestionBriefDto>();

                return new TestQuestionBriefDto(
                    entity.QuestionId,
                    entity.QuestionPosition,
                    question.QuestionType.Type,
                    question.DifficultyNavigation.Level,
                    question.Marks,
                    QuestionMapper.BuildContentPreview(question.Content),
                    question.QuestionImages.Count > 0,
                    subquestions);
            })
            .ToList();

        return new TestDetailDto(
            test.Id,
            test.FileName,
            test.SchoolId,
            test.School!.Name,
            test.ClassId,
            test.Class!.ClassName,
            test.SubjectId,
            test.Subject!.Name,
            test.TestTypeId,
            test.TestType!.Type,
            test.Sections,
            test.TimeDuration,
            test.MaximumMarks,
            questions,
            test.CreatedOn,
            test.UpdatedOn);
    }
}