using System;

namespace Domain.Massages;

public class NotificationMessage
{
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}