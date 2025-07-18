using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> AddAsync(CategoryDto dto);
    Task<CategoryDto> UpdateAsync(CategoryDto dto);
    Task<bool> DeleteAsync(int id);
} 