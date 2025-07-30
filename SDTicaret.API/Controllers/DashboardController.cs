using Microsoft.AspNetCore.Mvc;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.API.Models;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public DashboardController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
    {
        try
        {
            var stats = await _reportingService.GetDashboardStatsAsync();
            return Ok(new ApiResponse<DashboardStatsDto>
            {
                Success = true,
                Data = stats,
                Message = "Dashboard istatistikleri başarıyla getirildi."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<DashboardStatsDto>
            {
                Success = false,
                Message = "Dashboard istatistikleri alınırken bir hata oluştu: " + ex.Message
            });
        }
    }

    [HttpGet("recent-orders")]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetRecentOrders([FromQuery] int count = 5)
    {
        try
        {
            var orders = await _reportingService.GetRecentOrdersAsync(count);
            return Ok(new ApiResponse<List<OrderDto>>
            {
                Success = true,
                Data = orders,
                Message = "Son siparişler başarıyla getirildi."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<OrderDto>>
            {
                Success = false,
                Message = "Son siparişler alınırken bir hata oluştu: " + ex.Message
            });
        }
    }

    [HttpGet("low-stock-products")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetLowStockProducts([FromQuery] int count = 10)
    {
        try
        {
            var products = await _reportingService.GetLowStockProductsAsync(count);
            return Ok(new ApiResponse<List<ProductDto>>
            {
                Success = true,
                Data = products,
                Message = "Düşük stoklu ürünler başarıyla getirildi."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<ProductDto>>
            {
                Success = false,
                Message = "Düşük stoklu ürünler alınırken bir hata oluştu: " + ex.Message
            });
        }
    }

    [HttpGet("sales-chart")]
    public async Task<ActionResult<ApiResponse<SalesReportDto>>> GetSalesChart([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var salesData = await _reportingService.GetSalesReportAsync(startDate, endDate);
            return Ok(new ApiResponse<SalesReportDto>
            {
                Success = true,
                Data = salesData,
                Message = "Satış grafiği verileri başarıyla getirildi."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<SalesReportDto>
            {
                Success = false,
                Message = "Satış grafiği verileri alınırken bir hata oluştu: " + ex.Message
            });
        }
    }
} 