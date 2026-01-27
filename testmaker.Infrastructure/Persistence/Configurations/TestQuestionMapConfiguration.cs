using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class TestQuestionMapConfiguration : IEntityTypeConfiguration<TestQuestionMap>
{
    public void Configure(EntityTypeBuilder<TestQuestionMap> entity)
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("test_question_map");

        entity.HasIndex(e => e.QuestionId, "fk_test_ques_map_ques_id");

        entity.HasIndex(e => e.TestId, "fk_test_ques_map_test_id");

        entity.Property(e => e.Id)
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.QuestionId)
            .HasMaxLength(36)
            .HasColumnName("question_id");
        entity.Property(e => e.QuestionPosition).HasColumnName("question_position");
        entity.Property(e => e.TestId)
            .HasMaxLength(36)
            .HasColumnName("test_id");
        entity.Property(e => e.UpdatedOn)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");
    }
}