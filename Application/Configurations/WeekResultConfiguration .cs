using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Configurations;


public class WeekResultConfiguration : IEntityTypeConfiguration<WeekResult>
{
    public void Configure(EntityTypeBuilder<WeekResult> builder)
    {
        builder.HasKey(wr => wr.Id);

        
        builder.HasIndex(wr => new { wr.StudentId, wr.GroupId, wr.WeekNumber })
            .IsUnique();

        builder.Property(wr => wr.MentorComment)
            .HasMaxLength(1000);

        builder.Property(wr => wr.BonusScore)
            .HasDefaultValue(0);

        builder.Property(wr => wr.ExamScore)
            .HasDefaultValue(0);

        builder.Property(wr => wr.TotalScore)
            .HasDefaultValue(0);
    }
}