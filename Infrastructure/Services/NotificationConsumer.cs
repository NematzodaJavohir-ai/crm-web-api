
using Domain.Entities;
using Domain.Enums;
using Domain.Massages;
using Infrastructure.Data;
using MassTransit;

namespace Infrastructure.Consumers;

public class NotificationConsumer(DataContext dbContext) : IConsumer<NotificationMessage>
{
    public async Task Consume(ConsumeContext<NotificationMessage> consumeContext)
    {
        var notification = new Notification
        {
            UserId = consumeContext.Message.UserId,
            Title = consumeContext.Message.Title,
            Message = consumeContext.Message.Message,
            Type = Enum.Parse<NotificationType>(consumeContext.Message.Type),
            IsRead = false,
            CreatedAt = consumeContext.Message.CreatedAt
        };

        await dbContext.Notifications.AddAsync(notification);
        await dbContext.SaveChangesAsync();
    }
}