namespace SDTicaret.Core.Entities;

public class Campaign : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DiscountRate { get; set; }
    public bool IsActive { get; set; } = true;
} 
