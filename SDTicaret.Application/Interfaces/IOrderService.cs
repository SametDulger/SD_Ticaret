using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> AddAsync(OrderDto dto);
    Task<OrderDto> UpdateAsync(OrderDto dto);
    Task<bool> DeleteAsync(int id);
} 