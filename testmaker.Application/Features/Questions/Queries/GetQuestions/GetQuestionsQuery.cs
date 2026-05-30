using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Queries.GetQuestions;

public sealed record GetQuestionsQuery(
    Guid? ClassId,
    Guid? SubjectId,
    Guid? QuestionTypeId,
    Guid? DifficultyId,
    string? Search,
    int? Page,
    int? PageSize) : IRequest<Result<List<QuestionListItemDto>>>;