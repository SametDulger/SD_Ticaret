namespace SDTicaret.Application.DTOs;

public class StockDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public string? Location { get; set; }
    public bool IsLowStockAlertEnabled { get; set; } = true;
    public DateTime? LastStockInDate { get; set; }
    public DateTime? LastStockOutDate { get; set; }
    public int TotalStockIn { get; set; }
    public int TotalStockOut { get; set; }
    public string StockStatus { get; set; } = string.Empty; // "NORMAL", "LOW", "OUT", "OVERSTOCK"
    public bool IsLowStock { get; set; }
    public bool IsOutOfStock { get; set; }
    public bool IsOverstock { get; set; }
} 