using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;

namespace testmaker.Application.Features.Schools.Queries.GetAllSchools;

public class GetAllSchoolsQueryHandler : IRequestHandler<GetAllSchoolsQuery, Result<List<SchoolDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSchoolsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SchoolDto>>> Handle(GetAllSchoolsQuery request, CancellationToken cancellationToken)
    {
        var schools = await _context.Schools
            .OrderBy(s => s.Name)
            .Select(s => new SchoolDto(s.Id, s.Name, s.LogoFilename, s.CreatedOn, s.UpdatedOn))
            .ToListAsync(cancellationToken);

        return Result<List<SchoolDto>>.Success(schools);
    }
}
