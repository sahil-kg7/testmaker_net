using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;

namespace testmaker.Application.Features.Subjects.Queries.GetSubjectById;

public class GetSubjectByIdQueryHandler : IRequestHandler<GetSubjectByIdQuery, Result<SubjectDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSubjectByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SubjectDto>> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Subjects
            .Where(s => s.Id == request.Id)
            .Select(s => new SubjectDto(s.Id, s.Name, s.CreatedOn, s.UpdatedOn))
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return Result<SubjectDto>.Failure($"Subject with Id '{request.Id}' not found.", ErrorType.NotFound);

        return Result<SubjectDto>.Success(entity);
    }
}
