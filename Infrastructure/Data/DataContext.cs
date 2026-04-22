using Application.Configurations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options){}
    
    public DbSet<User> Users{get;set;}
    public DbSet<VerificationCode> VerificationCodes {get;set;}
    public DbSet<Role> Roles { get; set; }

    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<Student> Students { get; set; }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupStudent> GroupStudents { get; set; }

    // 4. Процесс и Успеваемость
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<WeekResult> WeekResults { get; set; }

    // 5. Система
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }

}
