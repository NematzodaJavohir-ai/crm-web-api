using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Infrastructure.Data;
namespace Infrastructure;

public class UnitOfWork(
    DataContext context,
    IGroupRepository groups,
    IStudentRepository students,
    IMentorRepository mentors,
    ICourseRepository courses,
    ILessonRepository lessons,
    IAttendanceRepository attendances,
    IWeekResultRepository weekResults,
    IGroupStudentRepository groupStudents) : IUnitOfWork
{
    public IGroupRepository Groups => groups;
    public ICourseRepository Courses => courses ;
    public IStudentRepository Students => students;
    public IMentorRepository Mentors => mentors;
    public ILessonRepository Lessons => lessons;
    public IAttendanceRepository Attendances => attendances;
    public IWeekResultRepository WeekResults => weekResults;
    public IGroupStudentRepository GroupStudents => groupStudents;

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync()
        => await context.Database.BeginTransactionAsync();

    public async Task CommitAsync()
        => await context.Database.CommitTransactionAsync();

    public async Task RollbackAsync()
        => await context.Database.RollbackTransactionAsync();
}