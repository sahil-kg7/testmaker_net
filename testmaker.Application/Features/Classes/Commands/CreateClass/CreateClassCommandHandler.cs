using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateClassCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var entity = new Class
        {
            Id = Guid.NewGuid(),
            ClassName = request.ClassName
        };

        _context.Classes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
