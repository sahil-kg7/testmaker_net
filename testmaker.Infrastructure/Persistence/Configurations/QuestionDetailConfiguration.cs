using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class QuestionDetailConfiguration : IEntityTypeConfiguration<QuestionDetail>
{
    public void Configure(EntityTypeBuilder<QuestionDetail> entity)
    {
        var jsonListConverter = new ValueConverter<List<string>?, string?>(
            value => value == null ? null : JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => string.IsNullOrWhiteSpace(value)
                ? null
                : JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null));

        var jsonListComparer = new ValueComparer<List<string>?>(
            (left, right) =>
                left == right ||
                (left != null && right != null && left.SequenceEqual(right)),
            value =>
                value == null
                    ? 0
                    : value.Aggregate(
                        0,
                        (current, item) => HashCode.Combine(
                            current,
                            item == null ? 0 : StringComparer.Ordinal.GetHashCode(item))),
            value => value == null ? null : value.ToList());

        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("question_details");

        entity.HasIndex(e => e.Difficulty, "fk_ques_details_difficulty");

        entity.HasIndex(e => e.SubjectId, "fk_ques_details_subject");

        entity.HasIndex(e => e.QuestionTypeId, "question_details_question_type_FK");

        entity.Property(e => e.Id)
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.Assertion).HasColumnName("assertion");
        entity.Property(e => e.Content).HasColumnName("content");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.Difficulty)
            .HasMaxLength(36)
            .HasColumnName("difficulty");
        entity.Property(e => e.FibWords)
            .HasConversion(jsonListConverter, jsonListComparer)
            .HasColumnType("json")
            .HasColumnName("fib_words");
        entity.Property(e => e.Marks).HasColumnName("marks");
        entity.Property(e => e.MatchA)
            .HasConversion(jsonListConverter, jsonListComparer)
            .HasColumnType("json")
            .HasColumnName("match_a");
        entity.Property(e => e.MatchB)
            .HasConversion(jsonListConverter, jsonListComparer)
            .HasColumnType("json")
            .HasColumnName("match_b");
        entity.Property(e => e.Mcq)
            .HasConversion(jsonListConverter, jsonListComparer)
            .HasColumnType("json")
            .HasColumnName("mcq");
        entity.Property(e => e.QuestionTypeId).HasColumnName("question_type_id");
        entity.Property(e => e.Reason).HasColumnName("reason");
        entity.Property(e => e.SubjectId)
            .HasMaxLength(36)
            .HasColumnName("subject_id");
        entity.Property(e => e.UpdatedOn)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");

        entity.HasOne(d => d.DifficultyNavigation).WithMany(p => p.QuestionDetails)
            .HasForeignKey(d => d.Difficulty)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("question_details_question_difficulty_FK");

        entity.HasOne(d => d.QuestionType).WithMany(p => p.QuestionDetails)
            .HasForeignKey(d => d.QuestionTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("question_details_question_type_FK");

        entity.HasOne(d => d.Subject).WithMany(p => p.QuestionDetails)
            .HasForeignKey(d => d.SubjectId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("question_details_subject_FK");
    }
}