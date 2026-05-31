using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Questions.Contracts;

namespace testmaker.Application.Features.Questions.Commands.CreateQuestion;

public sealed record CreateQuestionCommand(QuestionRequest Question) : IRequest<Result<QuestionDto>>;