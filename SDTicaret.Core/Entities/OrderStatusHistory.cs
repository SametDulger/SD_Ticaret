namespace SDTicaret.Core.Entities;

public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }
    public string PreviousStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? ChangedByUserId { get; set; }
    public string? ChangedByUserName { get; set; }
    public DateTime StatusChangeDate { get; set; }
} 