namespace SDTicaret.Core.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Manager { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public ICollection<Employee>? Employees { get; set; }
} 
