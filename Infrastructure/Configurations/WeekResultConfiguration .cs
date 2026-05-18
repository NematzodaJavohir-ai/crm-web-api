using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class WeekResultConfiguration : IEntityTypeConfiguration<WeekResult>
{
    public void Configure(EntityTypeBuilder<WeekResult> builder)
    {
        builder.HasKey(wr => wr.Id);

        builder.Property(wr => wr.MentorComment).HasMaxLength(1000);

        builder.HasOne(wr => wr.Student)
            .WithMany(s => s.WeekResults)
            .HasForeignKey(wr => wr.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wr => wr.Group)
            .WithMany(g => g.WeekResults)
            .HasForeignKey(wr => wr.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(wr => new { wr.StudentId, wr.GroupId, wr.WeekNumber }).IsUnique();
    }
}