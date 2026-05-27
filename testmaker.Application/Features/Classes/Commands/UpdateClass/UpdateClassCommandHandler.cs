using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;

namespace testmaker.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, Result<ClassDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateClassCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ClassDto>> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Classes
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity is null)
            return Result<ClassDto>.Failure($"Class with Id '{request.Id}' not found.", ErrorType.NotFound);

        var normalizedName = request.ClassName.ToLower();
        var duplicate = await _context.Classes
            .AnyAsync(c => c.ClassName.ToLower() == normalizedName && c.Id != request.Id, cancellationToken);

        if (duplicate)
            return Result<ClassDto>.Failure($"Class with name '{request.ClassName}' already exists.", ErrorType.Conflict);

        entity.ClassName = request.ClassName;
        entity.UpdatedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<ClassDto>.Success(new ClassDto(entity.Id, entity.ClassName));
    }
}
