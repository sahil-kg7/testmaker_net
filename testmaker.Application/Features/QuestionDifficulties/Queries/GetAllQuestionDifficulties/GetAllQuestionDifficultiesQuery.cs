using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.QuestionDifficulties.Queries.GetAllQuestionDifficulties;

public sealed record GetAllQuestionDifficultiesQuery : IRequest<Result<List<LookupDto>>>;