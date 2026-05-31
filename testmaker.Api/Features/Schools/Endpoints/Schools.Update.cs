using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Features.Schools.Models;
using testmaker.Application.Features.Schools.Commands.UpdateSchool;
using testmaker.Application.Features.Schools.Queries.GetAllSchools;

namespace testmaker.Api.Features.Schools.Endpoints;

public static class SchoolsUpdate
{
    public static RouteHandlerBuilder MapSchoolsUpdate(this IEndpointRouteBuilder app)
    {
        return app.MapPut("/{id:guid}", async (Guid id, UpdateSchoolRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateSchoolCommand(id, request.Name, request.LogoFilename);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : ErrorResult.From(result);
        })
        .Produces<SchoolDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .WithTags("Schools")
        .WithName("UpdateSchool");
    }
}
