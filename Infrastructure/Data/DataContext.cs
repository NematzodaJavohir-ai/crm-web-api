using Domain.Entities;
using Infrastructure.Configurations;
using Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupStudent> GroupStudents { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Homework> Homeworks { get; set; }
    public DbSet<LessonScore> LessonScores { get; set; }
    public DbSet<WeekResult> WeekResults { get; set; }
    public DbSet<StudentProgress> StudentProgresses { get; set; }
    public DbSet<Shedule> Shedules { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();

        builder.Properties<DateTime?>()
            .HaveConversion<UtcNullableDateTimeConverter>();
    }
}