using FluentValidation;

namespace testmaker.Application.Features.Questions.Common;

public sealed class QuestionPayloadValidator : AbstractValidator<QuestionPayload>
{
    public QuestionPayloadValidator()
    {
        RuleFor(payload => payload.QuestionTypeId)
            .NotEmpty().WithMessage("QuestionTypeId is required.");

        RuleFor(payload => payload.SubjectId)
            .NotEmpty().WithMessage("SubjectId is required.");

        RuleFor(payload => payload.ClassId)
            .NotEmpty().WithMessage("ClassId is required.");

        RuleFor(payload => payload.Difficulty)
            .NotEmpty().WithMessage("Difficulty is required.");

        RuleFor(payload => payload.Marks)
            .GreaterThan(0).WithMessage("Marks must be greater than 0.");

        RuleFor(payload => payload)
            .Must(payload =>
                !string.IsNullOrWhiteSpace(payload.Content) ||
                payload.Images is { Count: > 0 })
            .WithMessage("Content is required unless at least one image is provided.");

        RuleForEach(payload => payload.Images)
            .ChildRules(image =>
            {
                image.RuleFor(item => item.ImageName)
                    .NotEmpty().WithMessage("ImageName is required.")
                    .MaximumLength(50).WithMessage("ImageName must not exceed 50 characters.")
                    .Must(BeSafeImageName).WithMessage("ImageName must be a file name without path segments.");
            });

        RuleForEach(payload => payload.Mcq)
            .NotEmpty().WithMessage("MCQ values must not be empty.");

        RuleForEach(payload => payload.MatchA)
            .NotEmpty().WithMessage("MatchA values must not be empty.");

        RuleForEach(payload => payload.MatchB)
            .NotEmpty().WithMessage("MatchB values must not be empty.");

        RuleForEach(payload => payload.FibWords)
            .NotEmpty().WithMessage("FibWords values must not be empty.");
    }

    private static bool BeSafeImageName(string imageName)
    {
        return !imageName.Contains('/')
            && !imageName.Contains('\\')
            && !imageName.Contains("..");
    }
}