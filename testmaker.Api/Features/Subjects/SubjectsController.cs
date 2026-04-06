using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Features.Subjects.Commands.CreateSubject;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;

namespace testmaker.Api.Features.Subjects;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ISender _sender;

    public SubjectsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllSubjectsQuery(), cancellationToken);

        if (result.IsFailure)
            return Conflict(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return Conflict(new { error = result.Error });

        return CreatedAtAction(nameof(GetAll), new { id = result.Value!.Id }, result.Value);
    }
}
