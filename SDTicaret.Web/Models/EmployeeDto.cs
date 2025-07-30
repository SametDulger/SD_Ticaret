namespace SDTicaret.Web.Models;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public int BranchId { get; set; }
    public DateTime HireDate { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
} 