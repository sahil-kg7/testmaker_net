using testmaker.Api.Features.Tests.Endpoints;

namespace testmaker.Api.Features.Tests;

public static class TestEndpoints
{
    public static IEndpointRouteBuilder MapTestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tests");
        group.MapTestsGetAll();
        group.MapTestsGetById();
        group.MapTestsCreate();
        group.MapTestsUpdate();
        return app;
    }
}
