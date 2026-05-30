using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;

namespace testmaker.Application.Features.QuestionTypes.Queries.GetAllQuestionTypes;

public sealed class GetAllQuestionTypesQueryHandler : IRequestHandler<GetAllQuestionTypesQuery, Result<List<LookupDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllQuestionTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LookupDto>>> Handle(GetAllQuestionTypesQuery request, CancellationToken cancellationToken)
    {
        var questionTypes = await _context.QuestionTypes
            .AsNoTracking()
            .OrderBy(entity => entity.Type)
            .Select(entity => new LookupDto(entity.Id, entity.Type))
            .ToListAsync(cancellationToken);

        return Result<List<LookupDto>>.Success(questionTypes);
    }
}