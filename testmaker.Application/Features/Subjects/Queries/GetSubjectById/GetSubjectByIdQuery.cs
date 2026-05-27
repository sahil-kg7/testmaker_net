using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Subjects.Queries.GetAllSubjects;

namespace testmaker.Application.Features.Subjects.Queries.GetSubjectById;

public record GetSubjectByIdQuery(Guid Id) : IRequest<Result<SubjectDto>>;
