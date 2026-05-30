using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Tests.Common;

namespace testmaker.Application.Features.Tests.Queries.GetAllTests;

public sealed class GetAllTestsQueryHandler : IRequestHandler<GetAllTestsQuery, Result<List<TestListItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TestListItemDto>>> Handle(
        GetAllTestsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Tests
            .AsNoTracking()
            .Include(entity => entity.School)
            .Include(entity => entity.Class)
            .Include(entity => entity.Subject)
            .Include(entity => entity.TestType)
            .AsQueryable();

        if (request.SchoolId.HasValue)
        {
            query = query.Where(entity => entity.SchoolId == request.SchoolId.Value);
        }

        if (request.ClassId.HasValue)
        {
            query = query.Where(entity => entity.ClassId == request.ClassId.Value);
        }

        if (request.SubjectId.HasValue)
        {
            query = query.Where(entity => entity.SubjectId == request.SubjectId.Value);
        }

        query = query.OrderByDescending(entity => entity.UpdatedOn)
            .ThenBy(entity => entity.FileName);

        var page = request.Page.GetValueOrDefault(1);
        if (page < 1)
        {
            page = 1;
        }

        var pageSize = request.PageSize.GetValueOrDefault(50);
        if (pageSize < 1)
        {
            pageSize = 50;
        }

        pageSize = Math.Min(pageSize, 100);

        var skip = (page - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        var tests = await query.ToListAsync(cancellationToken);
        var testIds = tests.Select(entity => entity.Id).ToList();

        var questionCounts = testIds.Count == 0
            ? new Dictionary<Guid, int>()
            : await _context.TestQuestionMaps
                .AsNoTracking()
                .Where(entity => testIds.Contains(entity.TestId))
                .GroupBy(entity => entity.TestId)
                .Select(group => new { TestId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(entity => entity.TestId, entity => entity.Count, cancellationToken);

        var result = tests.Select(entity => new TestListItemDto(
                entity.Id,
                entity.FileName,
                entity.SchoolId,
                entity.School!.Name,
                entity.ClassId,
                entity.Class!.ClassName,
                entity.SubjectId,
                entity.Subject!.Name,
                entity.TestTypeId,
                entity.TestType!.Type,
                entity.TimeDuration,
                entity.MaximumMarks,
                questionCounts.GetValueOrDefault(entity.Id),
                entity.CreatedOn,
                entity.UpdatedOn))
            .ToList();

        return Result<List<TestListItemDto>>.Success(result);
    }
}