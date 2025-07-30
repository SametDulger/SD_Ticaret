namespace SDTicaret.Application.DTOs;

public class UpdateOrderStatusDto
{
    public int OrderId { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? ChangedByUserId { get; set; }
    public string? ChangedByUserName { get; set; }
    public string? TrackingNumber { get; set; }
    public string? CancellationReason { get; set; }
} 