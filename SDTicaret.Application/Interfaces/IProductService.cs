using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> AddAsync(ProductDto dto);
    Task<ProductDto> UpdateAsync(ProductDto dto);
    Task<bool> DeleteAsync(int id);
} 