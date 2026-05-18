using System;
using Application.Dtos.PaymentDto;
namespace Application.Dtos.StudentDto;

public class StudentResponseDto
{
   
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? PhotoUrl { get; set; }
    public string? TelegramUsername { get; set; }
    public string? GithubUrl { get; set; }
    public string? AboutMe { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime EnrollDate { get; set; }
    public int ActiveGroupsCount { get; set; }
}
public class StudentWithPaymentsDto : StudentResponseDto
{
    public IEnumerable<PaymentShortDto> Payments { get; set; } = new List<PaymentShortDto>();
}
