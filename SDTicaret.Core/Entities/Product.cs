namespace SDTicaret.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    // public Category Category { get; set; } // İleride eklenecek
    // public Supplier Supplier { get; set; } // İleride eklenecek
} 
