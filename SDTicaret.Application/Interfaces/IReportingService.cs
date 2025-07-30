using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IReportingService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<SalesReportDto> GetSalesReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<InventoryReportDto> GetInventoryReportAsync();
    Task<CustomerReportDto> GetCustomerReportAsync();
    Task<List<OrderDto>> GetRecentOrdersAsync(int count = 5);
    Task<List<ProductDto>> GetLowStockProductsAsync(int count = 10);
} 