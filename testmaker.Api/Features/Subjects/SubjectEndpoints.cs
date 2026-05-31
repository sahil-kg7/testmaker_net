using testmaker.Api.Features.Subjects.Endpoints;

namespace testmaker.Api.Features.Subjects;

public static class SubjectEndpoints
{
    public static IEndpointRouteBuilder MapSubjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/subjects");
        group.MapSubjectsGetAll();
        group.MapSubjectsGetById();
        group.MapSubjectsCreate();
        group.MapSubjectsUpdate();
        group.MapSubjectsDelete();
        return app;
    }
}
