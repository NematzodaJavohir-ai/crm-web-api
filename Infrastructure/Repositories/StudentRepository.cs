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
            .Include(s => s.User)
                .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.User)
                .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(s => s.UserId == userId, ct);

    public async Task<Student?> GetWithGroupsAsync(int id, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.User)
                .ThenInclude(u => u.Role)
            .Include(s => s.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Group)
                    .ThenInclude(g => g.Course)
            .Include(s => s.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Group)
                    .ThenInclude(g => g.Mentor)
                        .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Student?> GetWithPaymentsAsync(int id, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.User)
            .ThenInclude(u => u.Role)
            .Include(s => s.Payments.OrderByDescending(p => p.Date))
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Include(s => s.User)
                .ThenInclude(u => u.Role)
            .Include(s => s.GroupStudents.Where(gs => gs.IsActive))
                .ThenInclude(gs => gs.Group)
                    .ThenInclude(g => g.Course)
            .OrderBy(s => s.User.FirstName)
                .ThenBy(s => s.User.LastName)
            .ToListAsync(ct);

    public async Task<IEnumerable<Student>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Where(s => s.GroupStudents.Any(gs => gs.GroupId == groupId && gs.IsActive))
            .Include(s => s.User)
                .ThenInclude(u => u.Role)
            .Include(s => s.GroupStudents.Where(gs => gs.GroupId == groupId && gs.IsActive))
                .ThenInclude(gs => gs.Group)
                    .ThenInclude(g => g.Course)
            .OrderBy(s => s.User.FirstName)
                .ThenBy(s => s.User.LastName)
            .ToListAsync(ct);

    public async Task<IEnumerable<Student>> GetByBalanceAsync(decimal minBalance, CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Where(s => s.Balance >= minBalance)
            .Include(s => s.User)
            .ThenInclude(u => u.Role)
            .OrderBy(s => s.Balance)
            .ToListAsync(ct);

    public async Task<IEnumerable<Student>> GetDebtorsAsync(CancellationToken ct = default)
        => await context.Students
            .AsNoTracking()
            .Where(s => s.Balance < 0)
            .Include(s => s.User)
            .ThenInclude(u => u.Role)
            .Include(s => s.GroupStudents.Where(gs => gs.IsActive))
            .ThenInclude(gs => gs.Group)
            .ThenInclude(g => g.Course)
            .OrderBy(s => s.Balance)
            .ToListAsync(ct);

    public async Task<decimal> GetBalanceAsync(int id, CancellationToken ct = default)
    {
        var student = await context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return student?.Balance ?? 0;
    }

    public async Task UpdateBalanceAsync(int id, decimal amount, CancellationToken ct = default)
    {
        var student = await context.Students
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (student is null) return;

        student.Balance += amount;
        context.Students.Update(student);
    }
}