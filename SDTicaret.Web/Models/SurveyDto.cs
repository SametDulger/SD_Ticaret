namespace SDTicaret.Web.Models;

public class SurveyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime SurveyDate { get; set; }
    public int ParticipantCount { get; set; } = 1;
    public bool IsActive { get; set; } = true;
} 