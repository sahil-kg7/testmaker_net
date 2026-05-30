using FluentValidation;
using testmaker.Application.Features.Tests.Common;

namespace testmaker.Application.Features.Tests.Commands.CreateTest;

public sealed class CreateTestCommandValidator : AbstractValidator<CreateTestCommand>
{
    public CreateTestCommandValidator()
    {
        RuleFor(command => command.FileName)
            .NotEmpty().WithMessage("FileName is required.");

        RuleFor(command => command.SchoolId)
            .NotEmpty().WithMessage("SchoolId is required.");

        RuleFor(command => command.ClassId)
            .NotEmpty().WithMessage("ClassId is required.");

        RuleFor(command => command.SubjectId)
            .NotEmpty().WithMessage("SubjectId is required.");

        RuleFor(command => command.TestTypeId)
            .NotEmpty().WithMessage("TestTypeId is required.");

        RuleFor(command => command.TimeDuration)
            .GreaterThan(0).WithMessage("TimeDuration must be greater than 0.");

        RuleFor(command => command.MaximumMarks)
            .GreaterThan(0).WithMessage("MaximumMarks must be greater than 0.");

        RuleFor(command => command.Questions)
            .NotEmpty().WithMessage("At least one question is required.");

        RuleForEach(command => command.Questions)
            .SetValidator(new TestQuestionInputValidator());

        RuleFor(command => command.Sections)
            .Must((command, sections) =>
                sections is null ||
                (sections.Count == sections.Distinct().Count() && sections.All(section => section > 0 && section <= command.Questions.Count)))
            .WithMessage("Sections must contain unique values that map to question positions.");
    }
}