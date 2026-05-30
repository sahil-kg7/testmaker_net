using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Schools.Queries.GetAllSchools;

namespace testmaker.Application.Features.Schools.Queries.GetSchoolById;

public class GetSchoolByIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, Result<SchoolDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSchoolByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SchoolDto>> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Schools
            .Where(s => s.Id == request.Id)
            .Select(s => new SchoolDto(s.Id, s.Name, s.LogoFilename, s.CreatedOn, s.UpdatedOn))
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return Result<SchoolDto>.Failure($"School with Id '{request.Id}' not found.", ErrorType.NotFound);

        return Result<SchoolDto>.Success(entity);
    }
}
