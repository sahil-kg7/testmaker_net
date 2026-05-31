using testmaker.Api.Features.Schools.Endpoints;

namespace testmaker.Api.Features.Schools;

public static class SchoolEndpoints
{
    public static IEndpointRouteBuilder MapSchoolEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/schools");
        group.MapSchoolsGetAll();
        group.MapSchoolsGetById();
        group.MapSchoolsCreate();
        group.MapSchoolsUpdate();
        group.MapSchoolsDelete();
        return app;
    }
}
