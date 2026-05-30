using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Commands.UpdateQuestion;

public sealed record UpdateQuestionCommand(Guid Id, QuestionPayload Question) : IRequest<Result<QuestionDto>>;