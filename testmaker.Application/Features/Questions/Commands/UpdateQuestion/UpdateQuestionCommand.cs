using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Questions.Contracts;

namespace testmaker.Application.Features.Questions.Commands.UpdateQuestion;

public sealed record UpdateQuestionCommand(Guid Id, QuestionRequest Question) : IRequest<Result<QuestionDto>>;