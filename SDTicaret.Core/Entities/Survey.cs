namespace SDTicaret.Core.Entities;

public class Survey : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime SurveyDate { get; set; }
} 
