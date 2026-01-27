using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> entity)
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("school", tb => tb.HasComment("contains records of schools enlisted with app"));

        entity.Property(e => e.Id)
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.LogoFilename)
            .HasColumnType("text")
            .HasColumnName("logo_filename");
        entity.Property(e => e.Name)
            .HasMaxLength(50)
            .HasColumnName("name");
        entity.Property(e => e.UpdatedOn)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");
    }
}