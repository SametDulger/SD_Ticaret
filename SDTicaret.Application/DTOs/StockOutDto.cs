namespace SDTicaret.Application.DTOs;

public class StockOutDto
{
    public int StockId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? ReferenceNumber { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
} 