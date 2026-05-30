using MediatR;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Tests.Common;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Tests.Commands.CreateTest;

public sealed class CreateTestCommandHandler : IRequestHandler<CreateTestCommand, Result<TestDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateTestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TestDetailDto>> Handle(CreateTestCommand request, CancellationToken cancellationToken)
    {
        var referenceValidation = await TestContracts.ValidateReferencesAsync(
            request.SchoolId,
            request.ClassId,
            request.SubjectId,
            request.TestTypeId,
            _context,
            cancellationToken);

        if (referenceValidation.IsFailure)
        {
            return Result<TestDetailDto>.Failure(referenceValidation.Error!, referenceValidation.ErrorType);
        }

        var sectionsValidation = TestContracts.ValidateSections(request.Sections, request.Questions.Count);
        if (sectionsValidation.IsFailure)
        {
            return Result<TestDetailDto>.Failure(sectionsValidation.Error!, sectionsValidation.ErrorType);
        }

        var entity = new Test
        {
            Id = Guid.NewGuid(),
            FileName = request.FileName.Trim(),
            SchoolId = request.SchoolId,
            ClassId = request.ClassId,
            SubjectId = request.SubjectId,
            TestTypeId = request.TestTypeId,
            Sections = request.Sections?.ToList(),
            TimeDuration = request.TimeDuration,
            MaximumMarks = request.MaximumMarks
        };

        _context.Tests.Add(entity);

        var assemblyResult = await TestAssemblyBuilder.PopulateTestAsync(
            entity.Id,
            entity.ClassId,
            entity.SubjectId,
            request.Questions,
            _context,
            cancellationToken);

        if (assemblyResult.IsFailure)
        {
            return Result<TestDetailDto>.Failure(assemblyResult.Error!, assemblyResult.ErrorType);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var detail = await TestContracts.LoadTestDetailAsync(_context, entity.Id, cancellationToken);
        return Result<TestDetailDto>.Success(detail!);
    }
}