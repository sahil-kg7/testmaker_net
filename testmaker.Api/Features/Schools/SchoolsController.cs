using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Common;
using testmaker.Application.Features.Schools.Commands.CreateSchool;
using testmaker.Application.Features.Schools.Commands.DeleteSchool;
using testmaker.Application.Features.Schools.Commands.UpdateSchool;
using testmaker.Application.Features.Schools.Queries.GetAllSchools;
using testmaker.Application.Features.Schools.Queries.GetSchoolById;

namespace testmaker.Api.Features.Schools;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class SchoolsController : ControllerBase
{
    private readonly ISender _sender;

    public SchoolsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SchoolDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllSchoolsQuery(), cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SchoolDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSchoolByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);
        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSchoolResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return ErrorToHttp(result);

        return CreatedAtAction(nameof(GetAll), new { id = result.Value!.Id }, result.Value);
    }

    public record UpdateSchoolRequest(string Name, string? LogoFilename);

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SchoolDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSchoolRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSchoolCommand(id, request.Name, request.LogoFilename);
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
        var result = await _sender.Send(new DeleteSchoolCommand(id), cancellationToken);

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
