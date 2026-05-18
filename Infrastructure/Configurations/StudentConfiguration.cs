using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Balance).HasColumnType("decimal(18,2)");

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.GroupStudents)
            .WithOne(gs => gs.Student)
            .HasForeignKey(gs => gs.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Payments)
            .WithOne(p => p.Student)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Attendances)
            .WithOne(a => a.Student)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.WeekResults)
            .WithOne(wr => wr.Student)
            .HasForeignKey(wr => wr.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.EnrollDate);
        builder.HasIndex(s => s.Balance);
    }
}