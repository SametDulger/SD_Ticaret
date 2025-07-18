using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IOrderItemService
{
    Task<IEnumerable<OrderItemDto>> GetAllAsync();
    Task<OrderItemDto?> GetByIdAsync(int id);
    Task<OrderItemDto> AddAsync(OrderItemDto dto);
    Task<OrderItemDto> UpdateAsync(OrderItemDto dto);
    Task<bool> DeleteAsync(int id);
} 