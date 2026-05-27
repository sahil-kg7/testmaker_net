using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Common;
using testmaker.Application.Features.Classes.Commands.CreateClass;
using testmaker.Application.Features.Classes.Commands.DeleteClass;
using testmaker.Application.Features.Classes.Commands.UpdateClass;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;
using testmaker.Application.Features.Classes.Queries.GetClassById;

namespace testmaker.Api.Features.Classes;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ClassesController : ControllerBase
{
    private readonly ISender _sender;

    public ClassesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ClassDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllClassesQuery(), cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetClassByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateClassCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);

        return CreatedAtAction(nameof(GetAll), new { id = result.Value }, result.Value);
    }

    public record UpdateClassRequest(string ClassName);

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateClassCommand(id, request.ClassName);
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
        var result = await _sender.Send(new DeleteClassCommand(id), cancellationToken);

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
