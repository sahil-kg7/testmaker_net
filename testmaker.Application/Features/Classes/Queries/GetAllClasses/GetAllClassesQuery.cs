using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Classes.Queries.GetAllClasses;

public record GetAllClassesQuery : IRequest<Result<List<ClassDto>>>;
