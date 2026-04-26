using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Configurations;


public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .HasMaxLength(200);

        builder.Property(l => l.Description)
            .HasMaxLength(2000);

        builder.Property(l => l.HomeworkDescription)
            .HasMaxLength(2000);

        builder.Property(l => l.MaterialUrl)
            .HasMaxLength(300);

        
        builder.HasIndex(l => new { l.GroupId, l.LessonDate })
            .IsUnique();

        
        builder.HasMany(l => l.Attendances)
            .WithOne(a => a.Lesson)
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
