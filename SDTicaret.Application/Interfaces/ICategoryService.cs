using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> AddAsync(CategoryDto dto);
    Task<CategoryDto> UpdateAsync(CategoryDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<CategoryDto>> GetMainCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId);
    Task<IEnumerable<CategoryDto>> GetCategoryTreeAsync();
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
} 