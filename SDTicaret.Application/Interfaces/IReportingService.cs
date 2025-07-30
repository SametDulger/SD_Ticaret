using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IReportingService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate);
    Task<InventoryReportDto> GetInventoryReportAsync();
    Task<CustomerReportDto> GetCustomerReportAsync();
} 