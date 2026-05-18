using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class StudentProgressConfiguration : IEntityTypeConfiguration<StudentProgress>
{
    public void Configure(EntityTypeBuilder<StudentProgress> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.HasOne(sp => sp.Student)
            .WithMany()
            .HasForeignKey(sp => sp.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Group)
            .WithMany()
            .HasForeignKey(sp => sp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sp => new { sp.StudentId, sp.GroupId }).IsUnique();
    }
}