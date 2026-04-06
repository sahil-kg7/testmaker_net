using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Features.Classes.Commands.CreateClass;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;

namespace testmaker.Api.Features.Classes;

[ApiController]
[Route("api/[controller]")]
public class ClassesController : ControllerBase
{
    private readonly ISender _sender;

    public ClassesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllClassesQuery(), cancellationToken);

        if (result.IsFailure)
            return Conflict(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return Conflict(new { error = result.Error });

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}
