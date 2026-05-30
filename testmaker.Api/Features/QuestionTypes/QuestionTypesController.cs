using MediatR;
using Microsoft.AspNetCore.Mvc;
using testmaker.Application.Common;
using testmaker.Application.Features.QuestionTypes.Queries.GetAllQuestionTypes;

namespace testmaker.Api.Features.QuestionTypes;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class QuestionTypesController : ControllerBase
{
    private readonly ISender _sender;

    public QuestionTypesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<LookupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllQuestionTypesQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = result.Error });
        }

        return Ok(result.Value);
    }
}