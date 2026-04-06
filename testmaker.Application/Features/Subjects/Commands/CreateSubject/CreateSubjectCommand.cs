using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Subjects.Commands.CreateSubject;

public record CreateSubjectCommand(string Name) : IRequest<Result<CreateSubjectResponse>>;

public record CreateSubjectResponse(Guid Id, string Name, DateTime CreatedOn, DateTime UpdatedOn);
