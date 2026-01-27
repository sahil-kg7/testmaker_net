using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using testmaker.Domain.Entities;

namespace testmaker.Infrastructure.Persistence.Configurations;

public class QuestionImageConfiguration : IEntityTypeConfiguration<QuestionImage>
{
    public void Configure(EntityTypeBuilder<QuestionImage> entity)
    {
        entity.HasKey(e => e.Id).HasName("PRIMARY");

        entity.ToTable("question_images");

        entity.HasIndex(e => e.QuestionId, "fk_ques_images_question_id");

        entity.Property(e => e.Id)
            .HasMaxLength(36)
            .HasColumnName("id");
        entity.Property(e => e.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("created_on");
        entity.Property(e => e.ImageName)
            .HasMaxLength(50)
            .HasColumnName("image_name");
        entity.Property(e => e.ImagePosition).HasColumnName("image_position");
        entity.Property(e => e.QuestionId)
            .HasMaxLength(36)
            .HasColumnName("question_id");
        entity.Property(e => e.UpdatedOn)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("updated_on");

        entity.HasOne(d => d.Question).WithMany(p => p.QuestionImages)
            .HasForeignKey(d => d.QuestionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("question_images_question_details_FK");
    }
}