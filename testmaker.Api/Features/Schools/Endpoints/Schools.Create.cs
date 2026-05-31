using MediatR;
using testmaker.Api.Common;
using testmaker.Api.Features.Schools.Models;
using testmaker.Application.Features.Schools.Commands.CreateSchool;

namespace testmaker.Api.Features.Schools.Endpoints;

public static class SchoolsCreate
{
    public static RouteHandlerBuilder MapSchoolsCreate(this IEndpointRouteBuilder app)
    {
        return app.MapPost("/", async (CreateSchoolRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateSchoolCommand(request.Name, request.LogoFilename);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.CreatedAtRoute("GetSchoolById", new { id = result.Value!.Id }, result.Value)
                : ErrorResult.From(result);
        })
        .Produces<CreateSchoolResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status409Conflict)
        .WithTags("Schools")
        .WithName("CreateSchool");
    }
}
