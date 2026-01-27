using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class QuestionDifficultyConfiguration : IEntityTypeConfiguration<QuestionDifficulty>
{
    public void Configure(EntityTypeBuilder<QuestionDifficulty> entity)
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("question_difficulty");

        entity.Property(e => e.Id)
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.Level)
            .HasMaxLength(50)
            .HasColumnName("level");
        entity.Property(e => e.UpdatedOn)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");
    }
}