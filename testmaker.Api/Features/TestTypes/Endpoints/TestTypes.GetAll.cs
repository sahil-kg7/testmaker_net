using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Common;
using testmaker.Application.Features.TestTypes.Queries.GetAllTestTypes;

namespace testmaker.Api.Features.TestTypes.Endpoints;

public static class TestTypesGetAll
{
    public static RouteHandlerBuilder MapTestTypesGetAll(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllTestTypesQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<List<LookupDto>>(StatusCodes.Status200OK)
        .WithTags("TestTypes")
        .WithName("GetAllTestTypes");
    }
}
