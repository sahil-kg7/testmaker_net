using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Classes.Commands.DeleteClass;

public record DeleteClassCommand(Guid Id) : IRequest<Result>;
