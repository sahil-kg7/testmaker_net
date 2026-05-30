using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Schools.Queries.GetAllSchools;

public record GetAllSchoolsQuery : IRequest<Result<List<SchoolDto>>>;
