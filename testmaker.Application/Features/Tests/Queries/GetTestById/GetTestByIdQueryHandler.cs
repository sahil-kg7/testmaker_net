using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Tests.Common;

namespace testmaker.Application.Features.Tests.Queries.GetTestById;

public sealed class GetTestByIdQueryHandler : IRequestHandler<GetTestByIdQuery, Result<TestDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TestDetailDto>> Handle(GetTestByIdQuery request, CancellationToken cancellationToken)
    {
        var test = await TestContracts.LoadTestDetailAsync(_context, request.Id, cancellationToken);
        if (test is null)
        {
            return Result<TestDetailDto>.Failure(
                $"Test with Id '{request.Id}' not found.",
                ErrorType.NotFound);
        }

        return Result<TestDetailDto>.Success(test);
    }
}