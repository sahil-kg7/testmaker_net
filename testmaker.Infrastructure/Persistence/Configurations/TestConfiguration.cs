using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> entity)
    {
        var jsonListConverter = new ValueConverter<List<int>?, string?>(
            value => value == null ? null : JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => string.IsNullOrWhiteSpace(value)
                ? null
                : JsonSerializer.Deserialize<List<int>>(value, (JsonSerializerOptions?)null));

        var jsonListComparer = new ValueComparer<List<int>?>(
            (left, right) =>
                left == right ||
                (left != null && right != null && left.SequenceEqual(right)),
            value =>
                value == null
                    ? 0
                    : value.Aggregate(
                        0,
                        (current, item) => HashCode.Combine(current, item.GetHashCode())),
            value => value == null ? null : value.ToList());

        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("test");

        entity.HasIndex(e => e.ClassId, "fk_test_class_id");

        entity.HasIndex(e => e.SchoolId, "fk_test_school_id");

        entity.HasIndex(e => e.SubjectId, "fk_test_subject_id");

        entity.HasIndex(e => e.TestTypeId, "fk_test_type_id");

        entity.Property(e => e.Id)
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.ClassId)
            .HasMaxLength(36)
            .HasColumnName("class_id");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("UTC_TIMESTAMP()")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.FileName).HasColumnName("file_name");
        entity.Property(e => e.MaximumMarks).HasColumnName("maximum_marks");
        entity.Property(e => e.SchoolId)
            .HasMaxLength(36)
            .HasColumnName("school_id");
        entity.Property(e => e.Sections)
            .HasConversion(jsonListConverter, jsonListComparer)
            .HasColumnType("json")
            .HasColumnName("sections");
        entity.Property(e => e.SubjectId)
            .HasMaxLength(36)
            .HasColumnName("subject_id");
        entity.Property(e => e.TestTypeId)
            .HasMaxLength(36)
            .HasColumnName("test_type_id");
        entity.Property(e => e.TimeDuration).HasColumnName("time_duration");
        entity.Property(e => e.UpdatedOn)
            .HasDefaultValueSql("UTC_TIMESTAMP()")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");

        entity.HasOne(d => d.Class).WithMany(p => p.Tests)
            .HasForeignKey(d => d.ClassId)
            .HasConstraintName("test_class_FK");

        entity.HasOne(d => d.School).WithMany(p => p.Tests)
            .HasForeignKey(d => d.SchoolId)
            .HasConstraintName("test_school_FK");

        entity.HasOne(d => d.Subject).WithMany(p => p.Tests)
            .HasForeignKey(d => d.SubjectId)
            .HasConstraintName("test_subject_FK");

        entity.HasOne(d => d.TestType).WithMany(p => p.Tests)
            .HasForeignKey(d => d.TestTypeId)
            .HasConstraintName("test_test_type_FK");
    }
}