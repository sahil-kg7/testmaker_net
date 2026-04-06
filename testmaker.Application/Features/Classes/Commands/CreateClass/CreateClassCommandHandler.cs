using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateClassCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Classes
            .AnyAsync(c => c.ClassNumber == request.ClassNumber, cancellationToken);

        if (exists)
            return Result<int>.Failure($"Class with number {request.ClassNumber} already exists.");

        var entity = new Class
        {
            ClassNumber = request.ClassNumber,
            ClassRoman = request.ClassRoman
        };

        _context.Classes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(entity.ClassNumber);
    }
}
