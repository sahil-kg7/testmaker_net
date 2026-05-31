using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Tests.Common;
using testmaker.Application.Features.Tests.Queries.GetTestById;

namespace testmaker.Api.Features.Tests.Endpoints;

public static class TestsGetById
{
    public static RouteHandlerBuilder MapTestsGetById(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetTestByIdQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<TestDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Tests")
        .WithName("GetTestById");
    }
}
