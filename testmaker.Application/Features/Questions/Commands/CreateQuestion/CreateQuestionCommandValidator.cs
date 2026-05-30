using FluentValidation;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Commands.CreateQuestion;

public sealed class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(command => command.Question)
            .SetValidator(new QuestionPayloadValidator());
    }
}