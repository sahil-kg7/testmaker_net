using MediatR;
using testmaker.Application.Common;

namespace testmaker.Application.Features.Subjects.Queries.GetAllSubjects;

public record GetAllSubjectsQuery : IRequest<Result<List<SubjectDto>>>;
