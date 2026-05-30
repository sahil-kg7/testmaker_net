using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Queries.GetQuestionById;

public sealed class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, Result<QuestionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetQuestionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<QuestionDto>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await QuestionContracts.BuildDetailQuery(_context)
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);

        if (question is null)
        {
            return Result<QuestionDto>.Failure(
                $"Question with Id '{request.Id}' not found.",
                ErrorType.NotFound);
        }

        return Result<QuestionDto>.Success(QuestionContracts.ToQuestionDto(question));
    }
}