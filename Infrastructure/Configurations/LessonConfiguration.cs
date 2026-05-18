using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title).HasMaxLength(200);
        builder.Property(l => l.Description).HasMaxLength(2000);
        builder.Property(l => l.HomeworkDescription).HasMaxLength(2000);
        builder.Property(l => l.MaterialUrl).HasMaxLength(500);

        builder.HasOne(l => l.Group)
            .WithMany(g => g.Lessons)
            .HasForeignKey(l => l.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.LessonDate);
        builder.HasIndex(l => new { l.GroupId, l.WeekNumber });
    }
}