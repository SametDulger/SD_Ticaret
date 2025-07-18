namespace SDTicaret.Web.Models;

public class ContractDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CustomerId { get; set; }
    public int SupplierId { get; set; }
    public string? ContractType { get; set; }
} 