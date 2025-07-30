using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetDashboardStats()
    {
        try
        {
            var stats = await _reportingService.GetDashboardStatsAsync();
            return Ok(ApiResponse<DashboardStatsDto>.SuccessResult(stats));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<DashboardStatsDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("sales")]
    public async Task<ActionResult<ApiResponse<SalesReportDto>>> GetSalesReport(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
                return BadRequest(ApiResponse<SalesReportDto>.ErrorResult("Başlangıç tarihi bitiş tarihinden büyük olamaz"));

            var report = await _reportingService.GetSalesReportAsync(startDate, endDate);
            return Ok(ApiResponse<SalesReportDto>.SuccessResult(report));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SalesReportDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("inventory")]
    public async Task<ActionResult<ApiResponse<InventoryReportDto>>> GetInventoryReport()
    {
        try
        {
            var report = await _reportingService.GetInventoryReportAsync();
            return Ok(ApiResponse<InventoryReportDto>.SuccessResult(report));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InventoryReportDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("customers")]
    public async Task<ActionResult<ApiResponse<CustomerReportDto>>> GetCustomerReport()
    {
        try
        {
            var report = await _reportingService.GetCustomerReportAsync();
            return Ok(ApiResponse<CustomerReportDto>.SuccessResult(report));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CustomerReportDto>.ErrorResult(ex.Message));
        }
    }
} 