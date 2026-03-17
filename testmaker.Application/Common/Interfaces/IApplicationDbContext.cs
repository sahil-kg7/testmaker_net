using Microsoft.EntityFrameworkCore;
using testmaker.Domain.Entities;

namespace testmaker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Class> Classes { get; }

    DbSet<QuestionDetail> QuestionDetails { get; }

    DbSet<QuestionDifficulty> QuestionDifficulties { get; }

    DbSet<QuestionImage> QuestionImages { get; }

    DbSet<QuestionSubquestionMap> QuestionSubquestionMaps { get; }

    DbSet<QuestionType> QuestionTypes { get; }

    DbSet<School> Schools { get; }

    DbSet<Subject> Subjects { get; }

    DbSet<Test> Tests { get; }

    DbSet<TestQuestionMap> TestQuestionMaps { get; }

    DbSet<TestSectionMap> TestSectionMaps { get; }

    DbSet<TestType> TestTypes { get; }
}
