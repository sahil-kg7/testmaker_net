using FluentValidation;

namespace testmaker.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
    public UpdateClassCommandValidator()
    {
        RuleFor(x => x.ClassName)
            .NotEmpty().WithMessage("ClassName is required.")
            .MaximumLength(100).WithMessage("ClassName must not exceed 100 characters.");
    }
}
