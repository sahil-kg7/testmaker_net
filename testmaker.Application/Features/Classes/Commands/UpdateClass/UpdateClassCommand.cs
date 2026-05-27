using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;

namespace testmaker.Application.Features.Classes.Commands.UpdateClass;

public record UpdateClassCommand(Guid Id, string ClassName) : IRequest<Result<ClassDto>>;
