using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;

namespace testmaker.Application.Features.Subjects.Commands.UpdateSubject;

public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, Result<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SubjectDto>> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
            return Result<SubjectDto>.Failure($"Subject with Id '{request.Id}' not found.", ErrorType.NotFound);

        var normalizedName = request.Name.ToLower();
        var duplicate = await _context.Subjects
            .AnyAsync(s => s.Name.ToLower() == normalizedName && s.Id != request.Id, cancellationToken);

        if (duplicate)
            return Result<SubjectDto>.Failure($"Subject with name '{request.Name}' already exists.", ErrorType.Conflict);

        entity.Name = request.Name;
        entity.UpdatedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<SubjectDto>.Success(new SubjectDto(entity.Id, entity.Name, entity.CreatedOn, entity.UpdatedOn));
    }
}
