using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Features.Tests.Common;

namespace testmaker.Application.Features.Tests.Commands.CreateTest;

public sealed record CreateTestCommand(
    string FileName,
    Guid SchoolId,
    Guid ClassId,
    Guid SubjectId,
    Guid TestTypeId,
    IReadOnlyList<int>? Sections,
    int TimeDuration,
    int MaximumMarks,
    IReadOnlyList<TestQuestionInput> Questions) : IRequest<Result<TestDetailDto>>;