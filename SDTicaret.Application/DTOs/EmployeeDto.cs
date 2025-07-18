namespace SDTicaret.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public int BranchId { get; set; }
} 