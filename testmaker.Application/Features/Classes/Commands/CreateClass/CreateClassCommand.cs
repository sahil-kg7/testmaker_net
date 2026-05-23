using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Classes.Commands.CreateClass;

public record CreateClassCommand(string ClassName) : IRequest<Result<Guid>>;
