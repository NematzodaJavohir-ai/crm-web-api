using Application.Dtos.NotificationDto;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Enums;
using Domain.Massages;
using MassTransit;

namespace Infrastructure.Services;
/*
public class NotificationService(
    IBus bus,
    IUnitOfWork uow) : INotificationService
{
    public async Task SendAsync(int userId, string title, string message, NotificationType type, CancellationToken ct = default)
    {
        await bus.Publish(new NotificationMessage
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type.ToString(),
            CreatedAt = DateTime.UtcNow
        }, ct);
    }

    public async Task<Result<IEnumerable<NotificationResponseDto>>> GetMyNotificationsAsync(int userId, CancellationToken ct = default)
    {
        var notifications = await uow.Notifications.GetByUserIdAsync(userId, ct);

        var result = notifications.Select(n => new NotificationResponseDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        });

        return Result<IEnumerable<NotificationResponseDto>>.Ok(result);
    }

    public async Task<Result<UnreadCountDto>> GetUnreadCountAsync(int userId, CancellationToken ct = default)
    {
        var count = await uow.Notifications.GetUnreadCountAsync(userId, ct);

        return Result<UnreadCountDto>.Ok(new UnreadCountDto { Count = count });
    }

    public async Task<Result<bool>> MarkAsReadAsync(int notificationId, int userId, CancellationToken ct = default)
    {
        var notification = await uow.Notifications.GetByIdAsync(notificationId, ct);
        if (notification is null)
            return Result<bool>.Fail("Notification not found", ErrorType.NotFound);

        if (notification.UserId != userId)
            return Result<bool>.Fail("Forbidden", ErrorType.Forbidden);

        if (notification.IsRead)
            return Result<bool>.Fail("Notification already read", ErrorType.Validation);

        notification.IsRead = true;

        uow.Notifications.Update(notification);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> MarkAllAsReadAsync(int userId, CancellationToken ct = default)
    {
        var notifications = await uow.Notifications.GetUnreadByUserIdAsync(userId, ct);
        if (!notifications.Any())
            return Result<bool>.Fail("No unread notifications", ErrorType.NotFound);

        foreach (var notification in notifications)
            notification.IsRead = true;

        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> DeleteAsync(int notificationId, int userId, CancellationToken ct = default)
    {
        var notification = await uow.Notifications.GetByIdAsync(notificationId, ct);
        if (notification is null)
            return Result<bool>.Fail("Notification not found", ErrorType.NotFound);

        if (notification.UserId != userId)
            return Result<bool>.Fail("Forbidden", ErrorType.Forbidden);

        uow.Notifications.Delete(notification);
        await uow.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
*/