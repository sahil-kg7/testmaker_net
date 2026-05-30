using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Schools.Queries.GetAllSchools;

namespace testmaker.Application.Features.Schools.Commands.UpdateSchool;

public record UpdateSchoolCommand(Guid Id, string Name, string? LogoFilename) : IRequest<Result<SchoolDto>>;
