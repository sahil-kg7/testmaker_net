using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Queries.GetQuestionById;

public sealed record GetQuestionByIdQuery(Guid Id) : IRequest<Result<QuestionDto>>;