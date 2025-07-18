namespace SDTicaret.Core.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentType { get; set; } = null!;
    public string? PaymentMethod { get; set; }
    public string Status { get; set; } = "Pending";
} 
