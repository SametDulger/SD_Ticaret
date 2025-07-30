namespace SDTicaret.Application.DTOs;

public class CampaignDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal DiscountRate { get; set; }
    public bool IsActive { get; set; } = true;
} 