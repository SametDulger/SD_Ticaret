namespace SDTicaret.Application.DTOs;

public class ComplaintDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "Medium";
    public DateTime CreatedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public bool IsActive { get; set; } = true;
} 