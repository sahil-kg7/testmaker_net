using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Schools.Commands.DeleteSchool;

public record DeleteSchoolCommand(Guid Id) : IRequest<Result>;
