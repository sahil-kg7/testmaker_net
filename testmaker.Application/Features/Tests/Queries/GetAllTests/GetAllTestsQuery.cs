using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Tests.Common;

namespace testmaker.Application.Features.Tests.Queries.GetAllTests;

public sealed record GetAllTestsQuery(
    Guid? SchoolId,
    Guid? ClassId,
    Guid? SubjectId,
    int? Page,
    int? PageSize) : IRequest<Result<List<TestListItemDto>>>;