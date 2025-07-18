using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> AddAsync(CustomerDto dto);
    Task<CustomerDto> UpdateAsync(CustomerDto dto);
    Task<bool> DeleteAsync(int id);
} 