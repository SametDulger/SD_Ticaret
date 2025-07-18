namespace SDTicaret.Application.DTOs;

public class BranchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Manager { get; set; }
} 