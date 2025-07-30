namespace SDTicaret.Web.Models;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public CategoryDto? ParentCategory { get; set; }
    public ICollection<CategoryDto>? SubCategories { get; set; }
    public int ProductCount { get; set; }
} 