namespace Application.Dtos.ProfileDto;

public class ProfileResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? AvatarUrl { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? TelegramUsername { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GithubUrl { get; set; }
    public string? AboutMe { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}