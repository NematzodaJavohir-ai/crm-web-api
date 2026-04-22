using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Configurations;

public class StudentConfiguration:IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Phone)
            .HasMaxLength(20);

        builder.Property(s => s.AboutMe)
            .HasMaxLength(1000);

        builder.Property(s => s.GithubUrl)
            .HasMaxLength(300);

        builder.Property(s => s.TelegramUsername)
            .HasMaxLength(100);

        
        builder.HasMany(s => s.GroupStudents)
            .WithOne(gs => gs.Student)
            .HasForeignKey(gs => gs.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

       
        builder.HasMany(s => s.Attendances)
            .WithOne(a => a.Student)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        
        builder.HasMany(s => s.WeekResults)
            .WithOne(wr => wr.Student)
            .HasForeignKey(wr => wr.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
