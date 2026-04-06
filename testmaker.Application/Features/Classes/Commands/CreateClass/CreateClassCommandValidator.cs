using FluentValidation;

namespace testmaker.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
    public CreateClassCommandValidator()
    {
        RuleFor(x => x.ClassNumber)
            .GreaterThan(0).WithMessage("ClassNumber must be greater than 0.");

        RuleFor(x => x.ClassRoman)
            .NotEmpty().WithMessage("ClassRoman is required.");
    }
}
