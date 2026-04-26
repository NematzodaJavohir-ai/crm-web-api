using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudentRepository(DataContext context) : IStudentRepository
{
    public async Task AddAsync(Student student, CancellationToken ct = default)
        => await context.Students.AddAsync(student, ct);

    public void Update(Student student)
        => context.Students.Update(student);

    public void Delete(Student student)
        => context.Students.Remove(student);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Students.AnyAsync(s => s.Id == id, ct);

    public async Task<Student?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, ct);

    public async Task<Student?> GetWithUserAsync(int id, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Student?> GetWithGroupsAsync(int id, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Group)
                    .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Student?> GetWithAttendancesAsync(int id, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.Attendances)
                .ThenInclude(a => a.Lesson)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Student?> GetWithWeekResultsAsync(int id, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.WeekResults)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Student?> GetFullProfileAsync(int id, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Group)
                    .ThenInclude(g => g.Course)
            .Include(s => s.Attendances)
                .ThenInclude(a => a.Lesson)
            .Include(s => s.WeekResults)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .ToListAsync(ct);

    public async Task<IEnumerable<Student>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Where(s => s.GroupStudents.Any(gs => gs.GroupId == groupId && gs.IsActive))
            .Include(s => s.User)
            .ToListAsync(ct);
}