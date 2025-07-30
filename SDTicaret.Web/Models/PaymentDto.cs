namespace SDTicaret.Web.Models;

public class PaymentDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = "Pending";
    public bool IsActive { get; set; } = true;
} 