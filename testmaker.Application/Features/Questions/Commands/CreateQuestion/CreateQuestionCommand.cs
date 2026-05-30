using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Commands.CreateQuestion;

public sealed record CreateQuestionCommand(QuestionPayload Question) : IRequest<Result<QuestionDto>>;