using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Schools.Commands.CreateSchool;

public record CreateSchoolCommand(string Name, string? LogoFilename) : IRequest<Result<CreateSchoolResponse>>;

public record CreateSchoolResponse(Guid Id, string Name, string? LogoFilename, DateTime CreatedOn, DateTime UpdatedOn);
