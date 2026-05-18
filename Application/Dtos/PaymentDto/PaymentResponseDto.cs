namespace Application.Dtos.PaymentDto;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string Method { get; set; } = null!;
    public DateTime Date { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsConfirmed { get; set; }
    public string? Note { get; set; }
    public string? ReceiptUrl { get; set; }
}