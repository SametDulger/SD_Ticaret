namespace SDTicaret.Core.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public int BranchId { get; set; }
    public DateTime HireDate { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
} 
