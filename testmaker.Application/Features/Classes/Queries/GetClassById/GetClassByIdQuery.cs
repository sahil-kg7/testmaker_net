using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Classes.Queries.GetAllClasses;

namespace testmaker.Application.Features.Classes.Queries.GetClassById;

public record GetClassByIdQuery(Guid Id) : IRequest<Result<ClassDto>>;
