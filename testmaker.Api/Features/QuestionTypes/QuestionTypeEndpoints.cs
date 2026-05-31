using testmaker.Api.Features.QuestionTypes.Endpoints;

namespace testmaker.Api.Features.QuestionTypes;

public static class QuestionTypeEndpoints
{
    public static IEndpointRouteBuilder MapQuestionTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/question-types");
        group.MapQuestionTypesGetAll();
        return app;
    }
}
