namespace SDTicaret.Core.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public ICollection<Product>? Products { get; set; }
} 
