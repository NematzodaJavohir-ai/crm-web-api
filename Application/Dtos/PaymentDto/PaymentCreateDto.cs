using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dtos.PaymentDto;

public class PaymentCreateDto
{
    [Required(ErrorMessage = "StudentId is required")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "GroupId is required")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 9999999.99, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment type is required")]
    public PaymentType Type { get; set; } = PaymentType.FullPayment;

    [Required(ErrorMessage = "Payment method is required")]
    public PaymentMethod Method { get; set; } = PaymentMethod.Transfer;

    public DateTime? DueDate { get; set; }

    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    public string? Note { get; set; }

    [MaxLength(500, ErrorMessage = "ReceiptUrl cannot exceed 500 characters")]
    public string? ReceiptUrl { get; set; }
}