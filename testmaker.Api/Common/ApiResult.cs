using testmaker.Application.Common;

namespace testmaker.Api.Common;

public static class ErrorResult
{
    public static IResult From(Result result) => result.ErrorType switch
    {
        ErrorType.NotFound => Results.NotFound(new { error = result.Error }),
        ErrorType.Conflict => Results.Conflict(new { error = result.Error }),
        ErrorType.Validation => Results.BadRequest(new { error = result.Error }),
        _ => Results.Problem(detail: result.Error, statusCode: StatusCodes.Status500InternalServerError)
    };

    public static IResult NotFound(string error)
        => Results.NotFound(new { error });
}
