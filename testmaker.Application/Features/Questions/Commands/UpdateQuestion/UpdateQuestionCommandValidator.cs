using FluentValidation;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Commands.UpdateQuestion;

public sealed class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(command => command.Question)
            .SetValidator(new QuestionRequestValidator());
    }
}