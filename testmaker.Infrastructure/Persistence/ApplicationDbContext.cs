using Microsoft.EntityFrameworkCore;
using testmaker.Domain.Entities;
using testmaker.Application.Common.Interfaces;

namespace testmaker.Infrastructure.Persistence;

public partial class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Class> Classes => Set<Class>();

    public DbSet<QuestionDetail> QuestionDetails => Set<QuestionDetail>();

    public DbSet<QuestionDifficulty> QuestionDifficulties => Set<QuestionDifficulty>();

    public DbSet<QuestionImage> QuestionImages => Set<QuestionImage>();

    public DbSet<QuestionSubquestionMap> QuestionSubquestionMaps => Set<QuestionSubquestionMap>();

    public DbSet<QuestionType> QuestionTypes => Set<QuestionType>();

    public DbSet<School> Schools => Set<School>();

    public DbSet<Subject> Subjects => Set<Subject>();

    public DbSet<Test> Tests => Set<Test>();

    public DbSet<TestQuestionMap> TestQuestionMaps => Set<TestQuestionMap>();

    public DbSet<TestSectionMap> TestSectionMaps => Set<TestSectionMap>();
    public DbSet<TestType> TestTypes => Set<TestType>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Convert all Guid properties to string (varchar) in the database
        configurationBuilder
            .Properties<Guid>()
            .HaveConversion<string>()
            .HaveMaxLength(36);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
