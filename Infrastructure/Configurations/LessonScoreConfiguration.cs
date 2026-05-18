using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class LessonScoreConfiguration : IEntityTypeConfiguration<LessonScore>
{
    public void Configure(EntityTypeBuilder<LessonScore> builder)
    {
        builder.HasKey(ls => ls.Id);

        builder.Property(ls => ls.SubmissionUrl).HasMaxLength(500);
        builder.Property(ls => ls.MentorFeedback).HasMaxLength(1000);

        builder.HasOne(ls => ls.Homework)
            .WithMany(h => h.LessonScores)
            .HasForeignKey(ls => ls.HomeworkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ls => ls.Student)
            .WithMany()
            .HasForeignKey(ls => ls.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ls => new { ls.HomeworkId, ls.StudentId }).IsUnique();
    }
}