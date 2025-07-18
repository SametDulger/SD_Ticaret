namespace SDTicaret.Application.DTOs;

public class SurveyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CustomerId { get; set; }
} 