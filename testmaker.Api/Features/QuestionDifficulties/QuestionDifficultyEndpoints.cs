using testmaker.Api.Features.QuestionDifficulties.Endpoints;

namespace testmaker.Api.Features.QuestionDifficulties;

public static class QuestionDifficultyEndpoints
{
    public static IEndpointRouteBuilder MapQuestionDifficultyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/question-difficulties");
        group.MapQuestionDifficultiesGetAll();
        return app;
    }
}
