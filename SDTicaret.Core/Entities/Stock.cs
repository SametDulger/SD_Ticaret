namespace SDTicaret.Core.Entities;

public class Stock : BaseEntity
{
    public int ProductId { get; set; }
    public int BranchId { get; set; }
    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public string? Location { get; set; }
} 
