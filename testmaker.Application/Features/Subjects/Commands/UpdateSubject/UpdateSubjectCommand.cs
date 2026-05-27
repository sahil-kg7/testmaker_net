using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;

namespace testmaker.Application.Features.Subjects.Commands.UpdateSubject;

public record UpdateSubjectCommand(Guid Id, string Name) : IRequest<Result<SubjectDto>>;
