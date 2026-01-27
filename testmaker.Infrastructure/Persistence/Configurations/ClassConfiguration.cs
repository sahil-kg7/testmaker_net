using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.HasKey(e => e.ClassNumber).HasName("PRIMARY");

            builder.ToTable("class");

            builder.Property(e => e.ClassNumber).HasColumnName("class_number");
            builder.Property(e => e.ClassRoman)
                .HasMaxLength(50)
                .HasColumnName("class_roman");
            builder.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            builder.Property(e => e.UpdatedOn)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_on");
    }
}