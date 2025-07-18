using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAll()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResult(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResult("Kullanıcı bulunamadı"));

            return Ok(ApiResponse<UserDto>.SuccessResult(user));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create(UserDto dto)
    {
        try
        {
            var user = await _userService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, 
                ApiResponse<UserDto>.SuccessResult(user, "Kullanıcı başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(int id, UserDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<UserDto>.ErrorResult("ID uyumsuzluğu"));

            var user = await _userService.UpdateAsync(dto);
            return Ok(ApiResponse<UserDto>.SuccessResult(user, "Kullanıcı başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Kullanıcı başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}/activate")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(int id)
    {
        try
        {
            var result = await _userService.ActivateUserAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Kullanıcı başarıyla aktifleştirildi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}/deactivate")]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(int id)
    {
        try
        {
            var result = await _userService.DeactivateUserAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Kullanıcı başarıyla deaktifleştirildi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}/role")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeRole(int id, [FromBody] string role)
    {
        try
        {
            var result = await _userService.ChangeUserRoleAsync(id, role);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Kullanıcı bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Kullanıcı rolü başarıyla değiştirildi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("role/{role}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetByRole(string role)
    {
        try
        {
            var users = await _userService.GetByRoleAsync(role);
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResult(users));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResult(ex.Message));
        }
    }
} 