namespace SDTicaret.Application.DTOs;

public class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int ProductCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
} 