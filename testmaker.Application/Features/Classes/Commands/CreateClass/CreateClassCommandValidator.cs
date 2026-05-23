using FluentValidation;

namespace testmaker.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
    public CreateClassCommandValidator()
    {
        RuleFor(x => x.ClassName)
            .NotEmpty().WithMessage("ClassName is required.");
    }
}
