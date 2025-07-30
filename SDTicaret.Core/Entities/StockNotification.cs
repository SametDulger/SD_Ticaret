namespace SDTicaret.Core.Entities;

public class StockNotification : BaseEntity
{
    public int StockId { get; set; }
    public int ProductId { get; set; }
    public int BranchId { get; set; }
    public string NotificationType { get; set; } = null!; // "LOW_STOCK", "OUT_OF_STOCK", "OVERSTOCK"
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadDate { get; set; }
    public int? ReadByUserId { get; set; }
    public string? ReadByUserName { get; set; }
    public int CurrentQuantity { get; set; }
    public int ThresholdQuantity { get; set; }
} 