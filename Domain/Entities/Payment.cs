using Domain.Enums;
namespace Domain.Entities;



public class Payment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int GroupId { get; set; }
    public decimal Amount { get; set; }
    public PaymentType Type { get; set; } = PaymentType.FullPayment;
    public PaymentMethod Method { get; set; } = PaymentMethod.Transfer;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public bool IsConfirmed { get; set; } = false;
    public string? Note { get; set; }
    public string? ReceiptUrl { get; set; }

    public Student Student { get; set; } = null!;
    public Group Group { get; set; } = null!;
}

