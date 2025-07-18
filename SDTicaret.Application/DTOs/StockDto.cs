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
} 