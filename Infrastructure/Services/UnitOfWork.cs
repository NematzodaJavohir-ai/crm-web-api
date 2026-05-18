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
    IGroupStudentRepository groupStudents,
    IUserRepository users,
    //INotificationRepository notifications,
    IAuditLogRepository auditLogs,
    IHomeworkRepository homeworks,
    ILessonScoreRepository lessonScores,
    IPaymentRepository payments,
    IProfileRepository profiles,
    ISheduleRepository shedules,
    IStudentProgressRepository studentProgresses) : IUnitOfWork
{
    public IGroupRepository Groups => groups;
    public IStudentRepository Students => students;
    public IMentorRepository Mentors => mentors;
    public ICourseRepository Courses => courses;
    public ILessonRepository Lessons => lessons;
    public IAttendanceRepository Attendances => attendances;
    public IWeekResultRepository WeekResults => weekResults;
    public IGroupStudentRepository GroupStudents => groupStudents;
    public IUserRepository Users => users;
    //public INotificationRepository Notifications => notifications;
    public IAuditLogRepository AuditLogs => auditLogs;
    public IHomeworkRepository Homeworks => homeworks;
    public ILessonScoreRepository LessonScores => lessonScores;
    public IPaymentRepository Payments => payments;
    public IProfileRepository Profiles => profiles;
    public ISheduleRepository Shedules => shedules;
    public IStudentProgressRepository StudentProgresses => studentProgresses;

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync()
        => await context.Database.BeginTransactionAsync();

    public async Task CommitAsync()
        => await context.Database.CommitTransactionAsync();

    public async Task RollbackAsync()
        => await context.Database.RollbackTransactionAsync();
}