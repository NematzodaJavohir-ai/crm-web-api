using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Configurations;



public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.LessonId, a.StudentId })
            .IsUnique();

        builder.Property(a => a.Score)
            .HasDefaultValue(0);

        builder.Property(a => a.HomeworkScore)
            .HasDefaultValue(0);

        builder.Property(a => a.AbsenceReason)
            .HasMaxLength(1000);

        builder.Property(a => a.MentorNote)
            .HasMaxLength(500);
    }
}
