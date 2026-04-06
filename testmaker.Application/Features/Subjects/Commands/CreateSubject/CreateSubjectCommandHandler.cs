using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Subjects.Commands.CreateSubject;

public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, Result<CreateSubjectResponse>>
{
    private readonly IApplicationDbContext _context;

    public CreateSubjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateSubjectResponse>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.ToLower();

        var exists = await _context.Subjects
            .AnyAsync(s => s.Name.ToLower() == normalizedName, cancellationToken);

        if (exists)
            return Result<CreateSubjectResponse>.Failure($"Subject with name '{request.Name}' already exists.");

        var timestamp = DateTime.UtcNow;

        var entity = new Subject
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedOn = timestamp,
            UpdatedOn = timestamp
        };

        _context.Subjects.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateSubjectResponse>.Success(
            new CreateSubjectResponse(entity.Id, entity.Name, entity.CreatedOn, entity.UpdatedOn));
    }
}
