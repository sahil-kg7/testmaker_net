using MediatR;
using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Questions.Common;

namespace testmaker.Application.Features.Questions.Queries.GetQuestions;

public sealed class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, Result<List<QuestionListItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetQuestionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<QuestionListItemDto>>> Handle(
        GetQuestionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = QuestionContracts.BuildDetailQuery(_context);

        if (request.ClassId.HasValue)
        {
            query = query.Where(question => question.ClassId == request.ClassId.Value);
        }

        if (request.SubjectId.HasValue)
        {
            query = query.Where(question => question.SubjectId == request.SubjectId.Value);
        }

        if (request.QuestionTypeId.HasValue)
        {
            query = query.Where(question => question.QuestionTypeId == request.QuestionTypeId.Value);
        }

        if (request.DifficultyId.HasValue)
        {
            query = query.Where(question => question.Difficulty == request.DifficultyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = EscapeLikePattern(request.Search.Trim());
            var pattern = $"%{search}%";
            query = query.Where(question =>
                (question.Content != null && EF.Functions.Like(question.Content, pattern, "\\")) ||
                EF.Functions.Like(question.QuestionType.Type, pattern, "\\") ||
                EF.Functions.Like(question.Subject.Name, pattern, "\\") ||
                EF.Functions.Like(question.Class.ClassName, pattern, "\\"));
        }

        query = query.OrderByDescending(question => question.UpdatedOn)
            .ThenBy(question => question.Id);

        var page = request.Page.GetValueOrDefault(1);
        if (page < 1)
        {
            page = 1;
        }

        var pageSize = request.PageSize.GetValueOrDefault(50);
        if (pageSize < 1)
        {
            pageSize = 50;
        }

        pageSize = Math.Min(pageSize, 100);

        var skip = (page - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        var questions = await query.ToListAsync(cancellationToken);

        return Result<List<QuestionListItemDto>>.Success(
            questions.Select(QuestionContracts.ToQuestionListItemDto).ToList());
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }
}