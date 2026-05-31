using FluentValidation;
using testmaker.Application.Features.Questions.Contracts;

namespace testmaker.Application.Features.Questions.Common;

public sealed class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(request => request.QuestionTypeId)
            .NotEmpty().WithMessage("QuestionTypeId is required.");

        RuleFor(request => request.SubjectId)
            .NotEmpty().WithMessage("SubjectId is required.");

        RuleFor(request => request.ClassId)
            .NotEmpty().WithMessage("ClassId is required.");

        RuleFor(request => request.Difficulty)
            .NotEmpty().WithMessage("Difficulty is required.");

        RuleFor(request => request.Marks)
            .GreaterThan(0).WithMessage("Marks must be greater than 0.");

        RuleFor(request => request)
            .Must(request =>
                !string.IsNullOrWhiteSpace(request.Content) ||
                request.Images is { Count: > 0 })
            .WithMessage("Content is required unless at least one image is provided.");

        RuleForEach(request => request.Images)
            .ChildRules(image =>
            {
                image.RuleFor(item => item.ImageName)
                    .NotEmpty().WithMessage("ImageName is required.")
                    .MaximumLength(50).WithMessage("ImageName must not exceed 50 characters.")
                    .Must(BeSafeImageName).WithMessage("ImageName must be a file name without path segments.");
            });

        RuleForEach(request => request.Mcq)
            .NotEmpty().WithMessage("MCQ values must not be empty.");

        RuleForEach(request => request.MatchA)
            .NotEmpty().WithMessage("MatchA values must not be empty.");

        RuleForEach(request => request.MatchB)
            .NotEmpty().WithMessage("MatchB values must not be empty.");

        RuleForEach(request => request.FibWords)
            .NotEmpty().WithMessage("FibWords values must not be empty.");
    }

    private static bool BeSafeImageName(string imageName)
    {
        return !imageName.Contains('/')
            && !imageName.Contains('\\')
            && !imageName.Contains("..");
    }
}