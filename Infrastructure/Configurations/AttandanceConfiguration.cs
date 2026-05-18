using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AbsenceReason).HasMaxLength(500);
        builder.Property(a => a.MentorNote).HasMaxLength(500);

        builder.HasOne(a => a.Lesson)
            .WithMany(l => l.Attendances)
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Student)
            .WithMany(s => s.Attendances)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.LessonId, a.StudentId }).IsUnique();
    }
}