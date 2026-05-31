using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Schools.Queries.GetAllSchools;

namespace testmaker.Api.Features.Schools.Endpoints;

public static class SchoolsGetAll
{
    public static RouteHandlerBuilder MapSchoolsGetAll(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllSchoolsQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<List<SchoolDto>>(StatusCodes.Status200OK)
        .WithTags("Schools")
        .WithName("GetAllSchools");
    }
}
