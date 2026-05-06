using Application.Dtos.NotificationDto;
using Application.Results;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface INotificationService
{
    Task SendAsync(int userId, string title, string message, NotificationType type, CancellationToken ct = default);
    Task<Result<IEnumerable<NotificationResponseDto>>> GetMyNotificationsAsync(int userId, CancellationToken ct = default);
    Task<Result<UnreadCountDto>> GetUnreadCountAsync(int userId, CancellationToken ct = default);
    Task<Result<bool>> MarkAsReadAsync(int notificationId, int userId, CancellationToken ct = default);
    Task<Result<bool>> MarkAllAsReadAsync(int userId, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int notificationId, int userId, CancellationToken ct = default);
}