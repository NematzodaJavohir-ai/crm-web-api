namespace Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; } 
    public string EntityName { get; set; } = null!; 
    public string Action { get; set; } = null!; 
    public string OldValues { get; set; } = null!;
    public string NewValues { get; set; } = null!; 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
}