using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class NotificationRepository(DataContext context) : INotificationRepository
{
    public async Task AddAsync(Notification notification, CancellationToken ct = default)
        => await context.Notifications.AddAsync(notification, ct);

    public void Update(Notification notification)
        => context.Notifications.Update(notification);

    public void Delete(Notification notification)
        => context.Notifications.Remove(notification);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await context.Notifications.AnyAsync(n => n.Id == id, ct);

    public async Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default)
        => await context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, ct);

    public async Task<Notification?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Notifications
            .AsNoTracking()
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken ct = default)
        => await context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Notification>> GetByTypeAsync(NotificationType type, CancellationToken ct = default)
        => await context.Notifications
            .AsNoTracking()
            .Where(n => n.Type == type)
            .Include(n => n.User)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public async Task MarkAsReadAsync(int id, CancellationToken ct = default)
    {
        var notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, ct);

        if (notification is null) return;

        notification.IsRead = true;
        context.Notifications.Update(notification);
    }

    public async Task MarkAllAsReadAsync(int userId, CancellationToken ct = default)
    {
        var unread = await context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(ct);

        if (!unread.Any()) return;

        foreach (var n in unread)
            n.IsRead = true;

        context.Notifications.UpdateRange(unread);
    }
}