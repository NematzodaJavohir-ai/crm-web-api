using System;
using Application.Dtos.StudentDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IStudentService
{
    // ───── Basic CRUD ─────
    Task<Result<StudentResponseDto>> CreateAsync(StudentCreateDto dto, CancellationToken ct = default);
    Task<Result<StudentResponseDto>> UpdateAsync(int id, StudentUpdateDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<Result<StudentResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);

    // ───── Queries ─────
    Task<Result<IEnumerable<StudentResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<StudentResponseDto>>> GetActiveAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<StudentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);

    // ───── Detailed ─────
    Task<Result<StudentWithGroupsDto>> GetWithGroupsAsync(int id, CancellationToken ct = default);
    Task<Result<StudentWithPaymentsDto>> GetWithPaymentsAsync(int id, CancellationToken ct = default);
    Task<Result<StudentFullProfileDto>> GetFullProfileAsync(int id, CancellationToken ct = default);

    // ───── Admin Actions ─────
    Task<Result<StudentResponseDto>> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);

    // ───── Balance ─────
    Task<Result<decimal>> GetBalanceAsync(int id, CancellationToken ct = default);
    Task<Result<bool>> AddToBalanceAsync(int id, decimal amount, CancellationToken ct = default);
    Task<Result<bool>> DeductFromBalanceAsync(int id, decimal amount, CancellationToken ct = default);
    Task<Result<IEnumerable<StudentResponseDto>>> GetDebtorsAsync(CancellationToken ct = default);

    // ───── Student Profile ─────
    Task<Result<StudentResponseDto>> GetMyProfileAsync(int userId, CancellationToken ct = default);
    Task<Result<StudentFullProfileDto>> GetMyFullProfileAsync(int userId, CancellationToken ct = default);
    Task<Result<StudentResponseDto>> UpdateMyProfileAsync(int userId, StudentUpdateDto dto, CancellationToken ct = default);

    // ───── Lookup (dropdowns/modals) ─────
    Task<Result<IEnumerable<StudentShortDto>>> GetLookupAsync(CancellationToken ct = default);
    Task<Result<IEnumerable<StudentShortDto>>> GetLookupByGroupIdAsync(int groupId, CancellationToken ct = default);
}