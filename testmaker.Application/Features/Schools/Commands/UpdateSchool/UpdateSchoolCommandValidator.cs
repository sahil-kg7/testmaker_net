using FluentValidation;

namespace testmaker.Application.Features.Schools.Commands.UpdateSchool;

public class UpdateSchoolCommandValidator : AbstractValidator<UpdateSchoolCommand>
{
    public UpdateSchoolCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("School name is required.")
            .MaximumLength(50).WithMessage("School name must not exceed 50 characters.");
    }
}
