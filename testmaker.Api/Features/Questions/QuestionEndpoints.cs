using testmaker.Api.Features.Questions.Endpoints;

namespace testmaker.Api.Features.Questions;

public static class QuestionEndpoints
{
    public static IEndpointRouteBuilder MapQuestionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/questions");
        group.MapQuestionsGetAll();
        group.MapQuestionsGetById();
        group.MapQuestionsCreate();
        group.MapQuestionsUpdate();
        return app;
    }
}
