namespace SDTicaret.Application.DTOs;

public class ComplaintDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
} 