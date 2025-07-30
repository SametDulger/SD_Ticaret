namespace SDTicaret.Application.DTOs;

public class StockMovementDto
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public string MovementType { get; set; } = string.Empty; // "IN", "OUT", "ADJUSTMENT"
    public int Quantity { get; set; }
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public string? Reason { get; set; }
    public string? ReferenceNumber { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
} 