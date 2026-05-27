using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Common;
using testmaker.Application.Features.Subjects.Commands.CreateSubject;
using testmaker.Application.Features.Subjects.Commands.DeleteSubject;
using testmaker.Application.Features.Subjects.Commands.UpdateSubject;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;
using testmaker.Application.Features.Subjects.Queries.GetSubjectById;

namespace testmaker.Api.Features.Subjects;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class SubjectsController : ControllerBase
{
    private readonly ISender _sender;

    public SubjectsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SubjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllSubjectsQuery(), cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSubjectByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSubjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);

        return CreatedAtAction(nameof(GetAll), new { id = result.Value!.Id }, result.Value);
    }

    public record UpdateSubjectRequest(string Name);

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubjectRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSubjectCommand(id, request.Name);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);
        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteSubjectCommand(id), cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);
        return Ok();
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
