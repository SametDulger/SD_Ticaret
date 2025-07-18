using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> AddAsync(UserDto dto);
    Task<UserDto> UpdateAsync(UserDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ActivateUserAsync(int id);
    Task<bool> DeactivateUserAsync(int id);
    Task<bool> ChangeUserRoleAsync(int id, string role);
    Task<IEnumerable<UserDto>> GetByRoleAsync(string role);
} 