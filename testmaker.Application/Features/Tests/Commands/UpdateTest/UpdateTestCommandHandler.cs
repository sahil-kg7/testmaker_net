using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Tests.Common;

namespace testmaker.Application.Features.Tests.Commands.UpdateTest;

public sealed class UpdateTestCommandHandler : IRequestHandler<UpdateTestCommand, Result<TestDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TestDetailDto>> Handle(UpdateTestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tests
            .FirstOrDefaultAsync(test => test.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return Result<TestDetailDto>.Failure(
                $"Test with Id '{request.Id}' not found.",
                ErrorType.NotFound);
        }

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

        entity.FileName = request.FileName.Trim();
        entity.SchoolId = request.SchoolId;
        entity.ClassId = request.ClassId;
        entity.SubjectId = request.SubjectId;
        entity.TestTypeId = request.TestTypeId;
        entity.Sections = request.Sections?.ToList();
        entity.TimeDuration = request.TimeDuration;
        entity.MaximumMarks = request.MaximumMarks;

        var existingQuestionMaps = await _context.TestQuestionMaps
            .Where(map => map.TestId == entity.Id)
            .ToListAsync(cancellationToken);
        var existingSubquestionMaps = await _context.QuestionSubquestionMaps
            .Where(map => map.TestId == entity.Id)
            .ToListAsync(cancellationToken);

        if (existingQuestionMaps.Count > 0)
        {
            _context.TestQuestionMaps.RemoveRange(existingQuestionMaps);
        }

        if (existingSubquestionMaps.Count > 0)
        {
            _context.QuestionSubquestionMaps.RemoveRange(existingSubquestionMaps);
        }

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