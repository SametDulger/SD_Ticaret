using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IAuthService
{
    Task<TokenDto> LoginAsync(LoginDto loginDto);
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    Task<TokenDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<bool> ConfirmEmailAsync(string token);
    Task<bool> LogoutAsync(int userId);
    Task<UserDto?> GetCurrentUserAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId, UserDto userDto);
} 