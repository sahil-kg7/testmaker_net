using testmaker.Api.Features.Classes.Endpoints;

namespace testmaker.Api.Features.Classes;

public static class ClassEndpoints
{
    public static IEndpointRouteBuilder MapClassEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/classes");
        group.MapClassesGetAll();
        group.MapClassesGetById();
        group.MapClassesCreate();
        group.MapClassesUpdate();
        group.MapClassesDelete();
        return app;
    }
}
