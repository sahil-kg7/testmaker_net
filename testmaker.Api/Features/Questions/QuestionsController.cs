using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Common;
using testmaker.Application.Features.Questions.Commands.CreateQuestion;
using testmaker.Application.Features.Questions.Commands.UpdateQuestion;
using testmaker.Application.Features.Questions.Common;
using testmaker.Application.Features.Questions.Queries.GetQuestionById;
using testmaker.Application.Features.Questions.Queries.GetQuestions;

namespace testmaker.Api.Features.Questions;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class QuestionsController : ControllerBase
{
    private readonly ISender _sender;

    public QuestionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<QuestionListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? classId,
        [FromQuery] Guid? subjectId,
        [FromQuery] Guid? questionTypeId,
        [FromQuery] Guid? difficultyId,
        [FromQuery] string? search,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetQuestionsQuery(classId, subjectId, questionTypeId, difficultyId, search, page, pageSize),
            cancellationToken);

        if (result.IsFailure)
        {
            return ErrorToHttp(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetQuestionByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return ErrorToHttp(result);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] UpsertQuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateQuestionCommand(ToPayload(request)), cancellationToken);

        if (result.IsFailure)
        {
            return ErrorToHttp(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpsertQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateQuestionCommand(id, ToPayload(request)), cancellationToken);

        if (result.IsFailure)
        {
            return ErrorToHttp(result);
        }

        return Ok(result.Value);
    }

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