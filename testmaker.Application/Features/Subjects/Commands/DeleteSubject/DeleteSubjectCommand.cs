using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Subjects.Commands.DeleteSubject;

public record DeleteSubjectCommand(Guid Id) : IRequest<Result>;
