using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;

namespace testmaker.Application.Features.Classes.Queries.GetAllClasses;

public class GetAllClassesQueryHandler : IRequestHandler<GetAllClassesQuery, Result<List<ClassDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllClassesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ClassDto>>> Handle(GetAllClassesQuery request, CancellationToken cancellationToken)
    {
        var classes = await _context.Classes
            .OrderBy(c => c.ClassName)
            .Select(c => new ClassDto(c.Id, c.ClassName))
            .ToListAsync(cancellationToken);

        return Result<List<ClassDto>>.Success(classes);
    }
}
