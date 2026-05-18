using Application.Dtos.PaymentDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IPaymentService
{
    // ───── Basic CRUD ─────
    Task<Result<PaymentResponseDto>> CreateAsync(PaymentCreateDto dto, CancellationToken ct = default);
    Task<Result<PaymentResponseDto>> UpdateAsync(int id, PaymentUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<PaymentResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Queries ─────
    Task<Result<IEnumerable<PaymentResponseDto>>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);
    Task<Result<IEnumerable<PaymentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<PaymentResponseDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);

    // ───── Confirmation ─────
    Task<Result<PaymentResponseDto>> ConfirmAsync(int id, CancellationToken ct = default);
    Task<Result<PaymentResponseDto>> UnconfirmAsync(int id, CancellationToken ct = default);
    Task<Result<IEnumerable<PaymentResponseDto>>> GetUnconfirmedAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<PaymentResponseDto>>> GetByStatusAsync(bool isConfirmed, CancellationToken ct = default);

    // ───── Stats ─────
    Task<Result<decimal>> GetTotalPaidByStudentAsync(int studentId, CancellationToken ct = default);
    Task<Result<decimal>> GetTotalPaidByGroupAsync(int groupId, CancellationToken ct = default);
    Task<Result<decimal>> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<PaymentResponseDto>>> GetMyPaymentsAsync(int userId, CancellationToken ct = default);
    Task<Result<decimal>> GetMyTotalPaidAsync(int userId, CancellationToken ct = default);
}