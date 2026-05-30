using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.QuestionTypes.Queries.GetAllQuestionTypes;

public sealed record GetAllQuestionTypesQuery : IRequest<Result<List<LookupDto>>>;