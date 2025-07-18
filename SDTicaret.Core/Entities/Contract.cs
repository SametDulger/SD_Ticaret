namespace SDTicaret.Core.Entities;

public class Contract : BaseEntity
{
    public string Title { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CustomerId { get; set; }
    public int SupplierId { get; set; }
    public string? ContractType { get; set; }
    public decimal TotalValue { get; set; }
    public string Status { get; set; } = "Active";
} 
