using System.ComponentModel.DataAnnotations;

namespace SDTicaret.Core.Entities;

public class User : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = null!;
    
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = null!;
    
    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; } = null!;
    
    [StringLength(50)]
    public string? FirstName { get; set; }
    
    [StringLength(50)]
    public string? LastName { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool EmailConfirmed { get; set; } = false;
    
    public DateTime? LastLoginDate { get; set; }
    
    public string Role { get; set; } = "Customer"; // Admin, Employee, Customer
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiryTime { get; set; }
} 
