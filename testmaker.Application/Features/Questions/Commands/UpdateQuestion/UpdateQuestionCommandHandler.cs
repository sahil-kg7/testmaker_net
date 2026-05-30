using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Commands.UpdateQuestion;

public sealed class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result<QuestionDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateQuestionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<QuestionDto>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.QuestionDetails
            .Include(question => question.QuestionImages)
            .FirstOrDefaultAsync(question => question.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Result<QuestionDto>.Failure(
                $"Question with Id '{request.Id}' not found.",
                ErrorType.NotFound);
        }

        var referenceValidation = await QuestionContracts.ValidateReferencesAsync(
            request.Question,
            _context,
            cancellationToken);

        if (referenceValidation.IsFailure)
        {
            return Result<QuestionDto>.Failure(referenceValidation.Error!, referenceValidation.ErrorType);
        }

        var payloadResult = QuestionContracts.ApplyPayload(entity, request.Question, referenceValidation.Value!);
        if (payloadResult.IsFailure)
        {
            return Result<QuestionDto>.Failure(payloadResult.Error!, payloadResult.ErrorType);
        }

        if (entity.QuestionImages.Count > 0)
        {
            _context.QuestionImages.RemoveRange(entity.QuestionImages);
        }

        var images = QuestionContracts.CreateImageEntities(entity.Id, request.Question.Images);
        if (images.Count > 0)
        {
            _context.QuestionImages.AddRange(images);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var updatedQuestion = await QuestionContracts.BuildDetailQuery(_context)
            .FirstAsync(question => question.Id == entity.Id, cancellationToken);

        return Result<QuestionDto>.Success(QuestionContracts.ToQuestionDto(updatedQuestion));
    }
}