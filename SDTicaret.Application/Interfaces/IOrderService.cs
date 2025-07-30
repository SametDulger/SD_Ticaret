using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> AddAsync(OrderDto dto);
    Task<OrderDto> UpdateAsync(OrderDto dto);
    Task<bool> DeleteAsync(int id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderDto> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
    Task<bool> CancelOrderAsync(int orderId, string reason, int? userId = null, string? userName = null);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(int customerId);
    Task<IEnumerable<OrderStatusHistoryDto>> GetOrderStatusHistoryAsync(int orderId);
    Task<IEnumerable<OrderDto>> GetPendingOrdersAsync();
    Task<IEnumerable<OrderDto>> GetProcessingOrdersAsync();
    Task<IEnumerable<OrderDto>> GetShippedOrdersAsync();
} 