namespace SDTicaret.Application.DTOs;

public class StockNotificationDto
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public string NotificationType { get; set; } = string.Empty; // "LOW_STOCK", "OUT_OF_STOCK", "OVERSTOCK"
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadDate { get; set; }
    public int? ReadByUserId { get; set; }
    public string? ReadByUserName { get; set; }
    public int CurrentQuantity { get; set; }
    public int ThresholdQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
} 