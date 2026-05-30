using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Common;
using testmaker.Application.Features.TestTypes.Queries.GetAllTestTypes;

namespace testmaker.Api.Features.TestTypes;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TestTypesController : ControllerBase
{
    private readonly ISender _sender;

    public TestTypesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllTestTypesQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
        }

        return Ok(result.Value);
    }
}