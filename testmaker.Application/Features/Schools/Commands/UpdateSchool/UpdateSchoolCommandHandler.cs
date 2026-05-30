using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Schools.Queries.GetAllSchools;

namespace testmaker.Application.Features.Schools.Commands.UpdateSchool;

public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand, Result<SchoolDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SchoolDto>> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Schools
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
            return Result<SchoolDto>.Failure($"School with Id '{request.Id}' not found.", ErrorType.NotFound);

        var normalizedName = request.Name.ToLower();
        var duplicate = await _context.Schools
            .AnyAsync(s => s.Name.ToLower() == normalizedName && s.Id != request.Id, cancellationToken);

        if (duplicate)
            return Result<SchoolDto>.Failure($"School with name '{request.Name}' already exists.", ErrorType.Conflict);

        entity.Name = request.Name;
        entity.LogoFilename = request.LogoFilename;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<SchoolDto>.Success(new SchoolDto(entity.Id, entity.Name, entity.LogoFilename, entity.CreatedOn, entity.UpdatedOn));
    }
}
