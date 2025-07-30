using AutoMapper;
using Microsoft.Extensions.Configuration;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SDTicaret.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IJwtService jwtService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<TokenDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Repository<User>().GetAsync(u => u.Username == loginDto.Username && u.IsActive);
        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Kullanıcı adı veya şifre hatalı");

        user.LastLoginDate = DateTime.UtcNow;
        var refreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new TokenDto
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _unitOfWork.Repository<User>().GetAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Bu kullanıcı adı veya e-posta zaten kullanılıyor");

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Role = "Customer",
            IsActive = true,
            EmailConfirmed = false
        };

        await _unitOfWork.Repository<User>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<TokenDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        var user = await _unitOfWork.Repository<User>().GetAsync(u => u.RefreshToken == refreshTokenDto.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Geçersiz refresh token");

        var newRefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new TokenDto
        {
            AccessToken = _jwtService.GenerateAccessToken(user),
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
        if (user == null || !VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = _unitOfWork.Repository<User>().GetAsync(u => u.Email == forgotPasswordDto.Email).Result;
        if (user == null) return Task.FromResult(false);

        // Burada e-posta gönderme işlemi yapılacak
        // Şimdilik sadece true döndürüyoruz
        return Task.FromResult(true);
    }

    public Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        // Token doğrulama işlemi burada yapılacak
        // Şimdilik sadece true döndürüyoruz
        return Task.FromResult(true);
    }

    public Task<bool> ConfirmEmailAsync(string token)
    {
        // Token doğrulama işlemi burada yapılacak
        // Şimdilik sadece true döndürüyoruz
        return Task.FromResult(true);
    }

    public async Task<bool> LogoutAsync(int userId)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
        if (user == null) return false;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> UpdateProfileAsync(int userId, UserDto userDto)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
        if (user == null) return false;

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Email = userDto.Email;

        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
} 
