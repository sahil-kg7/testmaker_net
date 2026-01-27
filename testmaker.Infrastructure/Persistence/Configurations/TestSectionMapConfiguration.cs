using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class TestSectionMapConfiguration : IEntityTypeConfiguration<TestSectionMap>
{
    public void Configure(EntityTypeBuilder<TestSectionMap> entity)
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("test_section_map");

        entity.HasIndex(e => e.TestId, "fk_test_section_map_test_id");

        entity.Property(e => e.Id)
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.InitialQuesNumber).HasColumnName("initial_ques_number");
        entity.Property(e => e.SectionNumber).HasColumnName("section_number");
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