using testmaker.Api.Features.TestTypes.Endpoints;

namespace testmaker.Api.Features.TestTypes;

public static class TestTypeEndpoints
{
    public static IEndpointRouteBuilder MapTestTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/test-types");
        group.MapTestTypesGetAll();
        return app;
    }
}
