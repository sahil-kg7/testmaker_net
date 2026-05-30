using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Schools.Queries.GetAllSchools;

namespace testmaker.Application.Features.Schools.Queries.GetSchoolById;

public record GetSchoolByIdQuery(Guid Id) : IRequest<Result<SchoolDto>>;
