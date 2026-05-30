using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class QuestionTypeConfiguration : IEntityTypeConfiguration<QuestionType>
{
    public void Configure(EntityTypeBuilder<QuestionType> entity)
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("question_type");

        entity.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("UTC_TIMESTAMP()")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.Type)
            .HasMaxLength(50)
            .HasColumnName("type");
        entity.Property(e => e.UpdatedOn)
            .HasDefaultValueSql("UTC_TIMESTAMP()")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");
    }
}