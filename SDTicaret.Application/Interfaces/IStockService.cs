using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IStockService
{
    Task<IEnumerable<StockDto>> GetAllAsync();
    Task<StockDto?> GetByIdAsync(int id);
    Task<StockDto> AddAsync(StockDto dto);
    Task<StockDto> UpdateAsync(StockDto dto);
    Task<bool> DeleteAsync(int id);
    Task<StockDto> StockInAsync(StockInDto dto);
    Task<StockDto> StockOutAsync(StockOutDto dto);
    Task<IEnumerable<StockDto>> GetLowStockItemsAsync();
    Task<IEnumerable<StockDto>> GetOutOfStockItemsAsync();
    Task<IEnumerable<StockMovementDto>> GetStockMovementsAsync(int stockId);
    Task<IEnumerable<StockNotificationDto>> GetUnreadNotificationsAsync();
    Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId, string userName);
} 