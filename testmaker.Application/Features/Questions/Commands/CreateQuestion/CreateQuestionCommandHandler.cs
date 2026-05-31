using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Questions.Common;
using testmaker.Application.Features.Questions.Contracts;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Questions.Commands.CreateQuestion;

public sealed class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<QuestionDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateQuestionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<QuestionDto>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var referenceValidation = await QuestionValidator.ValidateReferencesAsync(
            request.Question,
            _context,
            cancellationToken);

        if (referenceValidation.IsFailure)
        {
            return Result<QuestionDto>.Failure(referenceValidation.Error!, referenceValidation.ErrorType);
        }

        var entity = new QuestionDetail
        {
            Id = Guid.NewGuid()
        };

        var payloadResult = QuestionValidator.ApplyRequest(entity, request.Question, referenceValidation.Value!);
        if (payloadResult.IsFailure)
        {
            return Result<QuestionDto>.Failure(payloadResult.Error!, payloadResult.ErrorType);
        }

        var images = QuestionValidator.CreateImageEntities(entity.Id, request.Question.Images);

        _context.QuestionDetails.Add(entity);
        if (images.Count > 0)
        {
            _context.QuestionImages.AddRange(images);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var createdQuestion = await QuestionMapper.BuildDetailQuery(_context)
            .FirstAsync(question => question.Id == entity.Id, cancellationToken);

        return Result<QuestionDto>.Success(QuestionMapper.ToDto(createdQuestion));
    }
}