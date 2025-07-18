using System.ComponentModel.DataAnnotations;

namespace SDTicaret.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    public string Username { get; set; } = null!;
    
    [Required(ErrorMessage = "Şifre gereklidir")]
    public string Password { get; set; } = null!;
}

public class RegisterDto
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır")]
    public string Username { get; set; } = null!;
    
    [Required(ErrorMessage = "E-posta gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Şifre gereklidir")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string Password { get; set; } = null!;
    
    [Required(ErrorMessage = "Şifre tekrarı gereklidir")]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmPassword { get; set; } = null!;
    
    [StringLength(50)]
    public string? FirstName { get; set; }
    
    [StringLength(50)]
    public string? LastName { get; set; }
}

public class TokenDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Mevcut şifre gereklidir")]
    public string CurrentPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "Yeni şifre gereklidir")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string NewPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "Şifre tekrarı gereklidir")]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmNewPassword { get; set; } = null!;
}

public class ForgotPasswordDto
{
    [Required(ErrorMessage = "E-posta gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; set; } = null!;
}

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; } = null!;
    
    [Required(ErrorMessage = "Yeni şifre gereklidir")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string NewPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "Şifre tekrarı gereklidir")]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmNewPassword { get; set; } = null!;
}

 