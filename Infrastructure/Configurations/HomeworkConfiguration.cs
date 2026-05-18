using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class HomeworkConfiguration : IEntityTypeConfiguration<Homework>
{
    public void Configure(EntityTypeBuilder<Homework> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Title).HasMaxLength(200).IsRequired();
        builder.Property(h => h.Description).HasMaxLength(2000).IsRequired();
        builder.Property(h => h.FileUrl).HasMaxLength(500);

        builder.HasOne(h => h.Lesson)
            .WithMany(l => l.Homeworks)
            .HasForeignKey(h => h.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => h.Deadline);
    }
}