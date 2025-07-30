namespace SDTicaret.Core.Entities;

public class StockMovement : BaseEntity
{
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public int BranchId { get; set; }
    public string MovementType { get; set; } = null!; // "IN", "OUT", "ADJUSTMENT"
    public int Quantity { get; set; }
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public string? Reason { get; set; }
    public string? ReferenceNumber { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
} 