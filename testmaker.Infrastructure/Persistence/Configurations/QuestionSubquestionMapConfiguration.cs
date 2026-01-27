using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class QuestionSubquestionMapConfiguration : IEntityTypeConfiguration<QuestionSubquestionMap>
{
    public void Configure(EntityTypeBuilder<QuestionSubquestionMap> entity)
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("question_subquestion_map");

        entity.HasIndex(e => e.QuestionId, "fk_ques_subques_map_ques_id");

        entity.HasIndex(e => e.SubquestionId, "fk_ques_subques_map_subques_id");

        entity.HasIndex(e => e.TestId, "fk_ques_subques_map_test_id");

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
        entity.Property(e => e.SubquestionId)
            .HasMaxLength(36)
            .HasColumnName("subquestion_id");
        entity.Property(e => e.SubquestionNumber).HasColumnName("subquestion_number");
        entity.Property(e => e.TestId)
            .HasMaxLength(36)
            .HasColumnName("test_id");
        entity.Property(e => e.UpdatedOn)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");

        entity.HasOne(d => d.Question).WithMany(p => p.QuestionSubquestionMapQuestions)
            .HasForeignKey(d => d.QuestionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("question_subquestion_map_question_details_FK");

        entity.HasOne(d => d.Subquestion).WithMany(p => p.QuestionSubquestionMapSubquestions)
            .HasForeignKey(d => d.SubquestionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("question_subquestion_map_question_details_FK_1");

        entity.HasOne(d => d.Test).WithMany(p => p.QuestionSubquestionMaps)
            .HasForeignKey(d => d.TestId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("question_subquestion_map_test_FK");
    }
}