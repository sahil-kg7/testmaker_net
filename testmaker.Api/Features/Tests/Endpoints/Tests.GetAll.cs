using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Tests.Common;
using testmaker.Application.Features.Tests.Queries.GetAllTests;

namespace testmaker.Api.Features.Tests.Endpoints;

public static class TestsGetAll
{
    public static RouteHandlerBuilder MapTestsGetAll(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", async (
            Guid? schoolId,
            Guid? classId,
            Guid? subjectId,
            int? page,
            int? pageSize,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetAllTestsQuery(schoolId, classId, subjectId, page, pageSize);
            var result = await sender.Send(query, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<List<TestListItemDto>>(StatusCodes.Status200OK)
        .WithTags("Tests")
        .WithName("GetAllTests");
    }
}
