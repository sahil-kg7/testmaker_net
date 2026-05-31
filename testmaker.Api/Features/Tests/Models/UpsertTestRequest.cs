using testmaker.Api.Common.Models;
using testmaker.Application.Features.Questions.Contracts;
using testmaker.Application.Features.Tests.Common;

namespace testmaker.Api.Features.Tests.Models;

public sealed record UpsertTestRequest(
    string FileName,
    Guid SchoolId,
    Guid ClassId,
    Guid SubjectId,
    Guid TestTypeId,
    IReadOnlyList<int>? Sections,
    int TimeDuration,
    int MaximumMarks,
    IReadOnlyList<TestQuestionRequest> Questions);

public sealed record TestQuestionRequest(
    Guid? ExistingQuestionId,
    UpsertQuestionRequest? NewQuestion,
    IReadOnlyList<TestSubquestionRequest>? SubQuestions);

public sealed record TestSubquestionRequest(
    Guid? ExistingQuestionId,
    UpsertQuestionRequest? NewQuestion);

internal static class TestMapping
{
    public static TestQuestionInput ToInput(this TestQuestionRequest request)
    {
        return new TestQuestionInput(
            request.ExistingQuestionId,
            request.NewQuestion?.ToRequest(),
            request.SubQuestions?.Select(ToInput).ToList());
    }

    public static TestSubquestionInput ToInput(this TestSubquestionRequest request)
    {
        return new TestSubquestionInput(
            request.ExistingQuestionId,
            request.NewQuestion?.ToRequest());
    }
}
