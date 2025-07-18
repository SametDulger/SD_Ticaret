namespace SDTicaret.Web.Models;

public class ComplaintDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
} 