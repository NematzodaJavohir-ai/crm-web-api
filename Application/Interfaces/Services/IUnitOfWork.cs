using System;
using Application.Interfaces.Repositories;

namespace Application.Interfaces.Services;

public interface IUnitOfWork
{
    IGroupRepository Groups { get; }
    IStudentRepository Students { get; }
    IMentorRepository Mentors { get; }
    ICourseRepository Courses { get; }
    ILessonRepository Lessons { get; }
    IAttendanceRepository Attendances { get; }
    IWeekResultRepository WeekResults { get; }
    IGroupStudentRepository GroupStudents { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
