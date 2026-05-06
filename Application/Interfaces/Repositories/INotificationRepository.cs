using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct = default);
    void Update(Notification notification);
    void Delete(Notification notification);

    Task<Notification?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}