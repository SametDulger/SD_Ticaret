namespace SDTicaret.Core.Entities;

public class Complaint : BaseEntity
{
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = "Open";
    public DateTime CreatedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
} 
