using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name).HasMaxLength(100).IsRequired();
        builder.Property(g => g.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(g => g.Course)
            .WithMany(c => c.Groups)
            .HasForeignKey(g => g.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Mentor)
            .WithMany(m => m.Groups)
            .HasForeignKey(g => g.MentorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(g => g.Name);
        builder.HasIndex(g => g.Status);
        builder.HasIndex(g => g.StartDate);
    }
}