using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;

namespace testmaker.Application.Features.TestTypes.Queries.GetAllTestTypes;

public sealed class GetAllTestTypesQueryHandler : IRequestHandler<GetAllTestTypesQuery, Result<List<LookupDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTestTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LookupDto>>> Handle(GetAllTestTypesQuery request, CancellationToken cancellationToken)
    {
        var testTypes = await _context.TestTypes
            .AsNoTracking()
            .OrderBy(entity => entity.Type)
            .Select(entity => new LookupDto(entity.Id, entity.Type))
            .ToListAsync(cancellationToken);

        return Result<List<LookupDto>>.Success(testTypes);
    }
}