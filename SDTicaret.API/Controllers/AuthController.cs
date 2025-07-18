using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using System.Security.Claims;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<TokenDto>>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(ApiResponse<TokenDto>.SuccessResult(result, "Giriş başarılı"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<TokenDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TokenDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(GetCurrentUser), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
        if (!result)
            return BadRequest(new { message = "Mevcut şifre hatalı" });

        return Ok(new { message = "Şifre başarıyla değiştirildi" });
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
        return Ok(new { message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi" });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);
        if (!result)
            return BadRequest(new { message = "Geçersiz veya süresi dolmuş token" });

        return Ok(new { message = "Şifre başarıyla sıfırlandı" });
    }

    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail([FromQuery] string token)
    {
        var result = await _authService.ConfirmEmailAsync(token);
        if (!result)
            return BadRequest(new { message = "Geçersiz veya süresi dolmuş token" });

        return Ok(new { message = "E-posta adresi başarıyla doğrulandı" });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        await _authService.LogoutAsync(userId);
        return Ok(new { message = "Başarıyla çıkış yapıldı" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(userId);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile([FromBody] UserDto userDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var result = await _authService.UpdateProfileAsync(userId, userDto);
        if (!result)
            return BadRequest(new { message = "Profil güncellenemedi" });

        return Ok(new { message = "Profil başarıyla güncellendi" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users/{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDto userDto)
    {
        var result = await _userService.AddAsync(userDto);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("users/{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UserDto userDto)
    {
        if (id != userDto.Id) return BadRequest();

        try
        {
            var result = await _userService.UpdateAsync(userDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("users/{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result) return NotFound();

        return Ok(new { message = "Kullanıcı başarıyla silindi" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{id}/activate")]
    public async Task<ActionResult> ActivateUser(int id)
    {
        var result = await _userService.ActivateUserAsync(id);
        if (!result) return NotFound();

        return Ok(new { message = "Kullanıcı başarıyla aktifleştirildi" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{id}/deactivate")]
    public async Task<ActionResult> DeactivateUser(int id)
    {
        var result = await _userService.DeactivateUserAsync(id);
        if (!result) return NotFound();

        return Ok(new { message = "Kullanıcı başarıyla deaktifleştirildi" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{id}/role")]
    public async Task<ActionResult> ChangeUserRole(int id, [FromBody] string role)
    {
        var result = await _userService.ChangeUserRoleAsync(id, role);
        if (!result) return NotFound();

        return Ok(new { message = "Kullanıcı rolü başarıyla değiştirildi" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users/role/{role}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string role)
    {
        var users = await _userService.GetByRoleAsync(role);
        return Ok(users);
    }
} 