using MediatR;
using testmaker.Api.Common;
using testmaker.Application.Features.Schools.Queries.GetAllSchools;
using testmaker.Application.Features.Schools.Queries.GetSchoolById;

namespace testmaker.Api.Features.Schools.Endpoints;

public static class SchoolsGetById
{
    public static RouteHandlerBuilder MapSchoolsGetById(this IEndpointRouteBuilder app)
    {
        return app.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSchoolByIdQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<SchoolDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Schools")
        .WithName("GetSchoolById");
    }
}
