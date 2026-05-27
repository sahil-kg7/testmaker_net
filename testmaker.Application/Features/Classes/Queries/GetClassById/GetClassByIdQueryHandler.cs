using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;

namespace testmaker.Application.Features.Classes.Queries.GetClassById;

public class GetClassByIdQueryHandler : IRequestHandler<GetClassByIdQuery, Result<ClassDto>>
{
    private readonly IApplicationDbContext _context;

    public GetClassByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ClassDto>> Handle(GetClassByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Classes
            .Where(c => c.Id == request.Id)
            .Select(c => new ClassDto(c.Id, c.ClassName))
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return Result<ClassDto>.Failure($"Class with Id '{request.Id}' not found.", ErrorType.NotFound);

        return Result<ClassDto>.Success(entity);
    }
}
