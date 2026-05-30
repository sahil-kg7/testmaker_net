using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Common;
using testmaker.Application.Features.Questions.Common;
using testmaker.Application.Features.Tests.Commands.CreateTest;
using testmaker.Application.Features.Tests.Commands.UpdateTest;
using testmaker.Application.Features.Tests.Common;
using testmaker.Application.Features.Tests.Queries.GetAllTests;
using testmaker.Application.Features.Tests.Queries.GetTestById;

namespace testmaker.Api.Features.Tests;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TestsController : ControllerBase
{
    private readonly ISender _sender;

    public TestsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TestListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? schoolId,
        [FromQuery] Guid? classId,
        [FromQuery] Guid? subjectId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllTestsQuery(schoolId, classId, subjectId, page, pageSize),
            cancellationToken);

        if (result.IsFailure)
        {
            return ErrorToHttp(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TestDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTestByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return ErrorToHttp(result);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TestDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UpsertTestRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateTestCommand(
                request.FileName,
                request.SchoolId,
                request.ClassId,
                request.SubjectId,
                request.TestTypeId,
                request.Sections,
                request.TimeDuration,
                request.MaximumMarks,
                request.Questions.Select(ToQuestionInput).ToList()),
            cancellationToken);

        if (result.IsFailure)
        {
            return ErrorToHttp(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TestDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpsertTestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateTestCommand(
                id,
                request.FileName,
                request.SchoolId,
                request.ClassId,
                request.SubjectId,
                request.TestTypeId,
                request.Sections,
                request.TimeDuration,
                request.MaximumMarks,
                request.Questions.Select(ToQuestionInput).ToList()),
            cancellationToken);

        if (result.IsFailure)
        {
            return ErrorToHttp(result);
        }

        return Ok(result.Value);
    }

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

    public sealed record TestSubquestionRequest(Guid? ExistingQuestionId, UpsertQuestionRequest? NewQuestion);

    public sealed record UpsertQuestionRequest(
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
        IReadOnlyList<QuestionImageRequest>? Images);

    public sealed record QuestionImageRequest(string ImageName);

    private static TestQuestionInput ToQuestionInput(TestQuestionRequest request)
    {
        return new TestQuestionInput(
            request.ExistingQuestionId,
            request.NewQuestion is null ? null : ToPayload(request.NewQuestion),
            request.SubQuestions?.Select(ToSubquestionInput).ToList());
    }

    private static TestSubquestionInput ToSubquestionInput(TestSubquestionRequest request)
    {
        return new TestSubquestionInput(
            request.ExistingQuestionId,
            request.NewQuestion is null ? null : ToPayload(request.NewQuestion));
    }

    private static QuestionPayload ToPayload(UpsertQuestionRequest request)
    {
        return new QuestionPayload(
            request.QuestionTypeId,
            request.SubjectId,
            request.ClassId,
            request.Difficulty,
            request.Marks,
            request.Content,
            request.Mcq,
            request.MatchA,
            request.MatchB,
            request.FibWords,
            request.Reason,
            request.Assertion,
            request.Images?.Select(image => new QuestionImageInput(image.ImageName)).ToList());
    }

    private IActionResult ErrorToHttp(Result result)
    {
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(new { error = result.Error }),
            ErrorType.Conflict => Conflict(new { error = result.Error }),
            ErrorType.Validation => BadRequest(new { error = result.Error }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error })
        };
    }
}