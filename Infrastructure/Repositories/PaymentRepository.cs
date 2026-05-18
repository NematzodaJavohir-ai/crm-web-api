using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PaymentRepository(DataContext context) : IPaymentRepository
{
    public async Task AddAsync(Payment payment, CancellationToken ct = default)
        => await context.Payments.AddAsync(payment, ct);

    public void Update(Payment payment)
        => context.Payments.Update(payment);

    public void Delete(Payment payment)
        => context.Payments.Remove(payment);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Payments.AnyAsync(p => p.Id == id, ct);

    public async Task<Payment?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Payments
            .AsNoTracking()
            .Include(p => p.Student)
                .ThenInclude(s => s.User)
            .Include(p => p.Group)
                .ThenInclude(g => g.Course)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Payment>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
        => await context.Payments
            .AsNoTracking()
            .Where(p => p.StudentId == studentId)
            .Include(p => p.Group)
                .ThenInclude(g => g.Course)
            .OrderByDescending(p => p.Date)
            .ToListAsync(ct);

    public async Task<IEnumerable<Payment>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
        => await context.Payments
            .AsNoTracking()
            .Where(p => p.GroupId == groupId)
            .Include(p => p.Student)
                .ThenInclude(s => s.User)
            .OrderByDescending(p => p.Date)
            .ToListAsync(ct);

    public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await context.Payments
            .AsNoTracking()
            .Where(p => p.Date >= from && p.Date <= to)
            .Include(p => p.Student)
                .ThenInclude(s => s.User)
            .Include(p => p.Group)
                .ThenInclude(g => g.Course)
            .OrderByDescending(p => p.Date)
            .ToListAsync(ct);

    public async Task<IEnumerable<Payment>> GetUnconfirmedAsync(CancellationToken ct = default)
        => await context.Payments
            .AsNoTracking()
            .Where(p => !p.IsConfirmed)
            .Include(p => p.Student)
                .ThenInclude(s => s.User)
            .Include(p => p.Group)
                .ThenInclude(g => g.Course)
            .OrderByDescending(p => p.Date)
            .ToListAsync(ct);

    public async Task<decimal> GetTotalPaidByStudentAsync(int studentId, CancellationToken ct = default)
        => await context.Payments
            .Where(p => p.StudentId == studentId && p.IsConfirmed)
            .SumAsync(p => p.Amount, ct);

    public async Task<decimal> GetTotalPaidByGroupAsync(int groupId, CancellationToken ct = default)
        => await context.Payments
            .Where(p => p.GroupId == groupId && p.IsConfirmed)
            .SumAsync(p => p.Amount, ct);

    public async Task<IEnumerable<Payment>> GetByStatusAsync(bool isConfirmed, CancellationToken ct = default)
        => await context.Payments
            .AsNoTracking()
            .Where(p => p.IsConfirmed == isConfirmed)
            .Include(p => p.Student)
                .ThenInclude(s => s.User)
            .Include(p => p.Group)
                .ThenInclude(g => g.Course)
            .OrderByDescending(p => p.Date)
            .ToListAsync(ct);
}