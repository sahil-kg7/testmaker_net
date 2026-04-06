using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Classes.Commands.CreateClass;

public record CreateClassCommand(int ClassNumber, string ClassRoman) : IRequest<Result<int>>;
