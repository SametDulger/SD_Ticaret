namespace SDTicaret.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<Product>? Products { get; set; }
} 
