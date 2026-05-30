using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.TestTypes.Queries.GetAllTestTypes;

public sealed record GetAllTestTypesQuery : IRequest<Result<List<LookupDto>>>;