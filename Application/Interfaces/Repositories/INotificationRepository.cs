using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct = default);
    void Update(Notification notification);
    void Delete(Notification notification);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    

    Task<Notification?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetByTypeAsync(NotificationType type, CancellationToken ct = default);
    Task MarkAsReadAsync(int id, CancellationToken ct = default);
    Task MarkAllAsReadAsync(int userId, CancellationToken ct = default);
}