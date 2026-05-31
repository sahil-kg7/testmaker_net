using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Common;
using testmaker.Application.Features.QuestionDifficulties.Queries.GetAllQuestionDifficulties;

namespace testmaker.Api.Features.QuestionDifficulties.Endpoints;

public static class QuestionDifficultiesGetAll
{
    public static RouteHandlerBuilder MapQuestionDifficultiesGetAll(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllQuestionDifficultiesQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<List<LookupDto>>(StatusCodes.Status200OK)
        .WithTags("QuestionDifficulties")
        .WithName("GetAllQuestionDifficulties");
    }
}
