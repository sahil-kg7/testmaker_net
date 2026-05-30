using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;

namespace testmaker.Application.Features.QuestionDifficulties.Queries.GetAllQuestionDifficulties;

public sealed class GetAllQuestionDifficultiesQueryHandler : IRequestHandler<GetAllQuestionDifficultiesQuery, Result<List<LookupDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllQuestionDifficultiesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LookupDto>>> Handle(GetAllQuestionDifficultiesQuery request, CancellationToken cancellationToken)
    {
        var difficulties = await _context.QuestionDifficulties
            .AsNoTracking()
            .OrderBy(entity => entity.Level)
            .Select(entity => new LookupDto(entity.Id, entity.Level))
            .ToListAsync(cancellationToken);

        return Result<List<LookupDto>>.Success(difficulties);
    }
}