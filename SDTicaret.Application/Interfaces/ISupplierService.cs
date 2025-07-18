using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface ISupplierService
{
    Task<IEnumerable<SupplierDto>> GetAllAsync();
    Task<SupplierDto?> GetByIdAsync(int id);
    Task<SupplierDto> AddAsync(SupplierDto dto);
    Task<SupplierDto> UpdateAsync(SupplierDto dto);
    Task<bool> DeleteAsync(int id);
} 