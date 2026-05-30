using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Tests.Common;

namespace testmaker.Application.Features.Tests.Queries.GetTestById;

public sealed record GetTestByIdQuery(Guid Id) : IRequest<Result<TestDetailDto>>;