using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.PaymentDto;

public class PaymentUpdateDto
{
    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    public string? Note { get; set; }

    [MaxLength(500, ErrorMessage = "ReceiptUrl cannot exceed 500 characters")]
    public string? ReceiptUrl { get; set; }
}