using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IStockService
{
    Task<IEnumerable<StockDto>> GetAllAsync();
    Task<StockDto?> GetByIdAsync(int id);
    Task<StockDto> AddAsync(StockDto dto);
    Task<StockDto> UpdateAsync(StockDto dto);
    Task<bool> DeleteAsync(int id);
} 