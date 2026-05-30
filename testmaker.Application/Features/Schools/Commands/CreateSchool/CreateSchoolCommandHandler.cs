using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Schools.Commands.CreateSchool;

public class CreateSchoolCommandHandler : IRequestHandler<CreateSchoolCommand, Result<CreateSchoolResponse>>
{
    private readonly IApplicationDbContext _context;

    public CreateSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateSchoolResponse>> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.ToLower();

        var exists = await _context.Schools
            .AnyAsync(s => s.Name.ToLower() == normalizedName, cancellationToken);

        if (exists)
            return Result<CreateSchoolResponse>.Failure($"School with name '{request.Name}' already exists.", ErrorType.Conflict);

        var entity = new School
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            LogoFilename = request.LogoFilename
        };

        _context.Schools.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateSchoolResponse>.Success(
            new CreateSchoolResponse(entity.Id, entity.Name, entity.LogoFilename, entity.CreatedOn, entity.UpdatedOn));
    }
}
