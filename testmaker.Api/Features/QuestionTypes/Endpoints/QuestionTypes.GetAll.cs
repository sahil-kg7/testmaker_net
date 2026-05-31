using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Common;
using testmaker.Application.Features.QuestionTypes.Queries.GetAllQuestionTypes;

namespace testmaker.Api.Features.QuestionTypes.Endpoints;

public static class QuestionTypesGetAll
{
    public static RouteHandlerBuilder MapQuestionTypesGetAll(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllQuestionTypesQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<List<LookupDto>>(StatusCodes.Status200OK)
        .WithTags("QuestionTypes")
        .WithName("GetAllQuestionTypes");
    }
}
