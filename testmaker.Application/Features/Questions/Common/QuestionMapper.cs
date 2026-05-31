using Microsoft.EntityFrameworkCore;
using testmaker.Application.Common.Interfaces;
using testmaker.Application.Features.Questions.Contracts;
using testmaker.Domain.Entities;

namespace testmaker.Application.Features.Questions.Common;

/// <summary>
/// Handles mapping between QuestionDetail entities and Question DTOs.
/// Also provides query building for question details with includes.
/// </summary>
internal static class QuestionMapper
{
    /// <summary>
    /// Builds a base query that includes all related entities needed for QuestionDto.
    /// Use this as the starting point for any query that returns question data.
    /// </summary>
    public static IQueryable<QuestionDetail> BuildDetailQuery(IApplicationDbContext context)
    {
        return context.QuestionDetails
            .AsNoTracking()
            .Include(question => question.QuestionType)
            .Include(question => question.Subject)
            .Include(question => question.Class)
            .Include(question => question.DifficultyNavigation)
            .Include(question => question.QuestionImages);
    }

    /// <summary>
    /// Maps a QuestionDetail entity to a QuestionDto.
    /// </summary>
    public static QuestionDto ToDto(QuestionDetail entity)
    {
        var orderedImages = entity.QuestionImages
            .OrderBy(image => image.ImagePosition)
            .Select(image => new QuestionImageDto(image.Id, image.ImageName, image.ImagePosition))
            .ToList();

        return new QuestionDto(
            entity.Id,
            entity.QuestionTypeId,
            entity.QuestionType.Type,
            entity.SubjectId,
            entity.Subject.Name,
            entity.ClassId,
            entity.Class.ClassName,
            entity.Difficulty,
            entity.DifficultyNavigation.Level,
            entity.Marks,
            entity.Content,
            entity.Mcq,
            entity.MatchA,
            entity.MatchB,
            entity.FibWords,
            entity.Reason,
            entity.Assertion,
            orderedImages,
            entity.CreatedOn,
            entity.UpdatedOn);
    }

    /// <summary>
    /// Maps a QuestionDetail entity to a QuestionListItemDto.
    /// </summary>
    public static QuestionListItemDto ToListItemDto(QuestionDetail entity)
    {
        return new QuestionListItemDto(
            entity.Id,
            entity.QuestionTypeId,
            entity.QuestionType.Type,
            entity.SubjectId,
            entity.Subject.Name,
            entity.ClassId,
            entity.Class.ClassName,
            entity.Difficulty,
            entity.DifficultyNavigation.Level,
            entity.Marks,
            BuildContentPreview(entity.Content),
            entity.QuestionImages.Count > 0,
            entity.CreatedOn,
            entity.UpdatedOn);
    }

    /// <summary>
    /// Builds a truncated preview of question content.
    /// </summary>
    public static string? BuildContentPreview(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        const int maxLength = 140;

        var normalized = content.Trim();
        if (normalized.Length <= maxLength)
        {
            return normalized;
        }

        return $"{normalized[..(maxLength - 3)]}...";
    }
}
