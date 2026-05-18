using System;

namespace Application.Dtos.PaymentDto;

public class PaymentShortDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string Method { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool IsConfirmed { get; set; }
    public string? GroupName { get; set; }
}