using Application.Dtos.GroupStudentDto;
using Application.Results;

namespace Application.Interfaces.Services;

public interface IGroupStudentService
{
    // ───── Basic Operations ─────
    Task<Result<GroupStudentResponseDto>> AddStudentAsync(AddStudentToGroupDto dto, CancellationToken ct = default);
    Task<Result<bool>> RemoveStudentAsync(RemoveStudentFromGroupDto dto, CancellationToken ct = default);
    Task<Result<GroupStudentResponseDto>> RestoreStudentAsync(int groupId, int studentId, CancellationToken ct = default);
    Task<Result<TransferResultDto>> TransferStudentAsync(TransferStudentDto dto, CancellationToken ct = default);

    // ───── Bulk Operations ─────
    Task<Result<BulkOperationResultDto>> BulkAddStudentsAsync(BulkAddStudentsDto dto, CancellationToken ct = default);
    Task<Result<BulkOperationResultDto>> BulkRemoveStudentsAsync(BulkRemoveStudentsDto dto, CancellationToken ct = default);

    // ───── Queries: Single ─────
    Task<Result<GroupStudentResponseDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Result<GroupStudentResponseDto>> GetActiveByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default);

    // ───── Queries: Lists ─────
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetActiveByGroupIdAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetByStudentAndGroupAsync(int studentId, int groupId, CancellationToken ct = default);

    // ───── History ─────
    Task<Result<IEnumerable<GroupStudentHistoryDto>>> GetStudentHistoryAsync(int studentId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupStudentHistoryDto>>> GetGroupHistoryAsync(int groupId, CancellationToken ct = default);
    Task<Result<IEnumerable<TransferHistoryDto>>> GetTransferHistoryAsync(int studentId, CancellationToken ct = default);
    Task<Result<IEnumerable<TransferHistoryDto>>> GetTransferHistoryByGroupAsync(int groupId, CancellationToken ct = default);

    // ───── Statistics ─────
    Task<Result<GroupStudentStatsDto>> GetGroupStatsAsync(int groupId, CancellationToken ct = default);
    Task<Result<int>> GetActiveStudentCountAsync(int groupId, CancellationToken ct = default);
    Task<Result<int>> GetTotalJoinedCountAsync(int groupId, CancellationToken ct = default);
    Task<Result<int>> GetTotalLeftCountAsync(int groupId, CancellationToken ct = default);
    Task<Result<bool>> IsStudentInGroupAsync(int studentId, int groupId, CancellationToken ct = default);
    Task<Result<bool>> HasAvailableSeatsAsync(int groupId, CancellationToken ct = default);

    // ───── Student ─────
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetMyGroupsAsync(int userId, CancellationToken ct = default);
    Task<Result<IEnumerable<GroupStudentResponseDto>>> GetMyActiveGroupsAsync(int userId, CancellationToken ct = default);
    Task<Result<int>> GetMyActiveGroupCountAsync(int userId, CancellationToken ct = default);
}