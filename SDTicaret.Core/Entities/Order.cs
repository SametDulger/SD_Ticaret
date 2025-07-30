namespace SDTicaret.Core.Entities;

public class Order : BaseEntity
{
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = "Pending"; // Pending, Confirmed, Processing, Shipped, Delivered, Cancelled
    public string? OrderNumber { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Notes { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? ProcessingDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? CancellationReason { get; set; }
    public int? AssignedEmployeeId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShippingMethod { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
    public ICollection<OrderStatusHistory>? StatusHistory { get; set; }
} 
