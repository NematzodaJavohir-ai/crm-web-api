using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; } 
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PhotoUrl { get; set; } 
    public string? RefreshToken { get; set; }  
    public DateTime? RefreshTokenExpiry { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public string? InviteToken { get; set; }
    public DateTime? InviteTokenExpiry { get; set; } 
    public bool IsPasswordSet { get; set; } = false;
      
    public InviteStatus InviteStatus => IsPasswordSet
        ? InviteStatus.Accepted
        : InviteTokenExpiry < DateTime.UtcNow
            ? InviteStatus.Expired
            : InviteStatus.Pending;

    // Navigation
    public Role Role { get; set; } = null!;
    public Mentor? Mentor { get; set; }
    public Student? Student { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}