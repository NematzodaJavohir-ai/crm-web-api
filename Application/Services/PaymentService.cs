using Application.Dtos.PaymentDto;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class PaymentService(IUnitOfWork uow) : IPaymentService
{
    public async Task<Result<PaymentResponseDto>> CreateAsync(PaymentCreateDto dto, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByIdAsync(dto.StudentId, ct);
        if (student is null)
            return Result<PaymentResponseDto>.Fail("Student not found", ErrorType.NotFound);

        var group = await uow.Groups.GetByIdAsync(dto.GroupId, ct);
        if (group is null)
            return Result<PaymentResponseDto>.Fail("Group not found", ErrorType.NotFound);

        var payment = new Payment
        {
            StudentId = dto.StudentId,
            GroupId = dto.GroupId,
            Amount = dto.Amount,
            Type = dto.Type,
            Method = dto.Method,
            Date = DateTime.UtcNow,
            DueDate = dto.DueDate,
            Note = dto.Note,
            ReceiptUrl = dto.ReceiptUrl,
            IsConfirmed = false
        };

        await uow.Payments.AddAsync(payment, ct);
        await uow.SaveChangesAsync(ct);

        var saved = await uow.Payments.GetByIdAsync(payment.Id, ct);
        return Result<PaymentResponseDto>.Ok(MapToResponseDto(saved!));
    }

    public async Task<Result<PaymentResponseDto>> UpdateAsync(int id, PaymentUpdateDto dto, CancellationToken ct = default)
    {
        var payment = await uow.Payments.GetByIdAsync(id, ct);
        if (payment is null)
            return Result<PaymentResponseDto>.Fail("Payment not found", ErrorType.NotFound);

        if (payment.IsConfirmed)
            return Result<PaymentResponseDto>.Fail("Cannot update confirmed payment", ErrorType.Validation);

        if (dto.Note is not null) payment.Note = dto.Note;
        if (dto.ReceiptUrl is not null) payment.ReceiptUrl = dto.ReceiptUrl;

        uow.Payments.Update(payment);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Payments.GetByIdAsync(id, ct);
        return Result<PaymentResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var payment = await uow.Payments.GetByIdAsync(id, ct);
        if (payment is null)
            return Result<bool>.Fail("Payment not found", ErrorType.NotFound);

        uow.Payments.Delete(payment);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<PaymentResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var payment = await uow.Payments.GetByIdAsync(id, ct);
        if (payment is null)
            return Result<PaymentResponseDto>.Fail("Payment not found", ErrorType.NotFound);

        return Result<PaymentResponseDto>.Ok(MapToResponseDto(payment));
    }

    public async Task<Result<IEnumerable<PaymentResponseDto>>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
    {
        var payments = await uow.Payments.GetByStudentIdAsync(studentId, ct);
        return Result<IEnumerable<PaymentResponseDto>>.Ok(payments.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<PaymentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        var payments = await uow.Payments.GetByGroupIdAsync(groupId, ct);
        return Result<IEnumerable<PaymentResponseDto>>.Ok(payments.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<PaymentResponseDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var payments = await uow.Payments.GetByDateRangeAsync(from, to, ct);
        return Result<IEnumerable<PaymentResponseDto>>.Ok(payments.Select(MapToResponseDto));
    }

    public async Task<Result<PaymentResponseDto>> ConfirmAsync(int id, CancellationToken ct = default)
    {
        var payment = await uow.Payments.GetByIdAsync(id, ct);
        if (payment is null)
            return Result<PaymentResponseDto>.Fail("Payment not found", ErrorType.NotFound);

        if (payment.IsConfirmed)
            return Result<PaymentResponseDto>.Fail("Payment already confirmed", ErrorType.Validation);

        payment.IsConfirmed = true;
        uow.Payments.Update(payment);

        await uow.Students.UpdateBalanceAsync(payment.StudentId, payment.Amount, ct);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Payments.GetByIdAsync(id, ct);
        return Result<PaymentResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<PaymentResponseDto>> UnconfirmAsync(int id, CancellationToken ct = default)
    {
        var payment = await uow.Payments.GetByIdAsync(id, ct);
        if (payment is null)
            return Result<PaymentResponseDto>.Fail("Payment not found", ErrorType.NotFound);

        if (!payment.IsConfirmed)
            return Result<PaymentResponseDto>.Fail("Payment is not confirmed", ErrorType.Validation);

        payment.IsConfirmed = false;
        uow.Payments.Update(payment);

        await uow.Students.UpdateBalanceAsync(payment.StudentId, -payment.Amount, ct);
        await uow.SaveChangesAsync(ct);

        var updated = await uow.Payments.GetByIdAsync(id, ct);
        return Result<PaymentResponseDto>.Ok(MapToResponseDto(updated!));
    }

    public async Task<Result<IEnumerable<PaymentResponseDto>>> GetUnconfirmedAsync(CancellationToken ct = default)
    {
        var payments = await uow.Payments.GetUnconfirmedAsync(ct);
        return Result<IEnumerable<PaymentResponseDto>>.Ok(payments.Select(MapToResponseDto));
    }

    public async Task<Result<IEnumerable<PaymentResponseDto>>> GetByStatusAsync(bool isConfirmed, CancellationToken ct = default)
    {
        var payments = await uow.Payments.GetByStatusAsync(isConfirmed, ct);
        return Result<IEnumerable<PaymentResponseDto>>.Ok(payments.Select(MapToResponseDto));
    }

    public async Task<Result<decimal>> GetTotalPaidByStudentAsync(int studentId, CancellationToken ct = default)
    {
        var total = await uow.Payments.GetTotalPaidByStudentAsync(studentId, ct);
        return Result<decimal>.Ok(total);
    }

    public async Task<Result<decimal>> GetTotalPaidByGroupAsync(int groupId, CancellationToken ct = default)
    {
        var total = await uow.Payments.GetTotalPaidByGroupAsync(groupId, ct);
        return Result<decimal>.Ok(total);
    }

    public async Task<Result<decimal>> GetTotalRevenueAsync(DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var fromDate = from ?? DateTime.MinValue;
        var toDate = to ?? DateTime.MaxValue;
        var payments = await uow.Payments.GetByDateRangeAsync(fromDate, toDate, ct);
        return Result<decimal>.Ok(payments.Where(p => p.IsConfirmed).Sum(p => p.Amount));
    }

    public async Task<Result<IEnumerable<PaymentResponseDto>>> GetMyPaymentsAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<IEnumerable<PaymentResponseDto>>.Fail("Student not found", ErrorType.NotFound);

        var payments = await uow.Payments.GetByStudentIdAsync(student.Id, ct);
        return Result<IEnumerable<PaymentResponseDto>>.Ok(payments.Select(MapToResponseDto));
    }

    public async Task<Result<decimal>> GetMyTotalPaidAsync(int userId, CancellationToken ct = default)
    {
        var student = await uow.Students.GetByUserIdAsync(userId, ct);
        if (student is null)
            return Result<decimal>.Fail("Student not found", ErrorType.NotFound);

        var total = await uow.Payments.GetTotalPaidByStudentAsync(student.Id, ct);
        return Result<decimal>.Ok(total);
    }

    private static PaymentResponseDto MapToResponseDto(Payment p) => new()
    {
        Id = p.Id,
        StudentId = p.StudentId,
        StudentName = p.Student?.User != null
            ? $"{p.Student.User.FirstName} {p.Student.User.LastName}"
            : "",
        GroupId = p.GroupId,
        GroupName = p.Group?.Name ?? "",
        Amount = p.Amount,
        Type = p.Type.ToString(),
        Method = p.Method.ToString(),
        Date = p.Date,
        DueDate = p.DueDate,
        IsConfirmed = p.IsConfirmed,
        Note = p.Note,
        ReceiptUrl = p.ReceiptUrl
    };
}