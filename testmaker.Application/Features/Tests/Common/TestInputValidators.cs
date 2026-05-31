using FluentValidation;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Tests.Common;

internal sealed class TestSubquestionInputValidator : AbstractValidator<TestSubquestionInput>
{
    public TestSubquestionInputValidator()
    {
        RuleFor(input => input)
            .Must(HasSingleQuestionSource)
            .WithMessage("Each sub-question must specify exactly one of existingQuestionId or newQuestion.");

        When(input => input.NewQuestion is not null, () =>
        {
            RuleFor(input => input.NewQuestion!)
                .SetValidator(new QuestionRequestValidator());
        });
    }

    private static bool HasSingleQuestionSource(TestSubquestionInput input)
    {
        return input.ExistingQuestionId.HasValue != (input.NewQuestion is not null);
    }
}

internal sealed class TestQuestionInputValidator : AbstractValidator<TestQuestionInput>
{
    public TestQuestionInputValidator()
    {
        RuleFor(input => input)
            .Must(HasSingleQuestionSource)
            .WithMessage("Each question must specify exactly one of existingQuestionId or newQuestion.");

        When(input => input.NewQuestion is not null, () =>
        {
            RuleFor(input => input.NewQuestion!)
                .SetValidator(new QuestionRequestValidator());
        });

        RuleForEach(input => input.SubQuestions)
            .SetValidator(new TestSubquestionInputValidator());
    }

    private static bool HasSingleQuestionSource(TestQuestionInput input)
    {
        return input.ExistingQuestionId.HasValue != (input.NewQuestion is not null);
    }
}