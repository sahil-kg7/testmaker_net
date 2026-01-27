using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> entity)
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("test");

        entity.HasIndex(e => e.ClassNumber, "fk_test_class_number");

        entity.HasIndex(e => e.SchoolId, "fk_test_school_id");

        entity.HasIndex(e => e.SubjectId, "fk_test_subject_id");

        entity.HasIndex(e => e.TestTypeId, "fk_test_type_id");

        entity.Property(e => e.Id)
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.ClassNumber).HasColumnName("class_number");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.FileName).HasColumnName("file_name");
        entity.Property(e => e.MaximumMarks).HasColumnName("maximum_marks");
        entity.Property(e => e.SchoolId)
            .HasMaxLength(36)
            .HasColumnName("school_id");
        entity.Property(e => e.SectionCount).HasColumnName("section_count");
        entity.Property(e => e.SubjectId)
            .HasMaxLength(36)
            .HasColumnName("subject_id");
        entity.Property(e => e.TestTypeId)
            .HasMaxLength(36)
            .HasColumnName("test_type_id");
        entity.Property(e => e.TimeDuration).HasColumnName("time_duration");
        entity.Property(e => e.UpdatedOn)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");

        entity.HasOne(d => d.ClassNumberNavigation).WithMany(p => p.Tests)
            .HasForeignKey(d => d.ClassNumber)
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