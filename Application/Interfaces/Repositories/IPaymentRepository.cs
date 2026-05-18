using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken ct = default);
    void Update(Payment payment);
    void Delete(Payment payment);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
  

    Task<Payment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Payment>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);
    Task<IEnumerable<Payment>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<IEnumerable<Payment>> GetUnconfirmedAsync(CancellationToken ct = default);
    Task<decimal> GetTotalPaidByStudentAsync(int studentId, CancellationToken ct = default);
    Task<decimal> GetTotalPaidByGroupAsync(int groupId, CancellationToken ct = default);
    Task<IEnumerable<Payment>> GetByStatusAsync(bool isConfirmed, CancellationToken ct = default);
}