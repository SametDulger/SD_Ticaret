using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockDto>>>> GetAll()
    {
        try
        {
            var stocks = await _stockService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<StockDto>>.SuccessResult(stocks));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<StockDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockDto>>>> GetLowStockItems()
    {
        try
        {
            var stocks = await _stockService.GetLowStockItemsAsync();
            return Ok(ApiResponse<IEnumerable<StockDto>>.SuccessResult(stocks));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<StockDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("out-of-stock")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockDto>>>> GetOutOfStockItems()
    {
        try
        {
            var stocks = await _stockService.GetOutOfStockItemsAsync();
            return Ok(ApiResponse<IEnumerable<StockDto>>.SuccessResult(stocks));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<StockDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StockDto>>> GetById(int id)
    {
        try
        {
            var stock = await _stockService.GetByIdAsync(id);
            if (stock == null)
                return NotFound(ApiResponse<StockDto>.ErrorResult("Stok bulunamadı"));

            return Ok(ApiResponse<StockDto>.SuccessResult(stock));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<StockDto>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}/movements")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockMovementDto>>>> GetStockMovements(int id)
    {
        try
        {
            var movements = await _stockService.GetStockMovementsAsync(id);
            return Ok(ApiResponse<IEnumerable<StockMovementDto>>.SuccessResult(movements));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<StockMovementDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("notifications/unread")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockNotificationDto>>>> GetUnreadNotifications()
    {
        try
        {
            var notifications = await _stockService.GetUnreadNotificationsAsync();
            return Ok(ApiResponse<IEnumerable<StockNotificationDto>>.SuccessResult(notifications));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<StockNotificationDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<StockDto>>> Create(StockDto dto)
    {
        try
        {
            var stock = await _stockService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = stock.Id }, 
                ApiResponse<StockDto>.SuccessResult(stock, "Stok başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<StockDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost("stock-in")]
    public async Task<ActionResult<ApiResponse<StockDto>>> StockIn(StockInDto dto)
    {
        try
        {
            var stock = await _stockService.StockInAsync(dto);
            return Ok(ApiResponse<StockDto>.SuccessResult(stock, "Stok girişi başarıyla yapıldı"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<StockDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost("stock-out")]
    public async Task<ActionResult<ApiResponse<StockDto>>> StockOut(StockOutDto dto)
    {
        try
        {
            var stock = await _stockService.StockOutAsync(dto);
            return Ok(ApiResponse<StockDto>.SuccessResult(stock, "Stok çıkışı başarıyla yapıldı"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<StockDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<StockDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<StockDto>>> Update(int id, StockDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<StockDto>.ErrorResult("ID uyumsuzluğu"));

            var stock = await _stockService.UpdateAsync(dto);
            return Ok(ApiResponse<StockDto>.SuccessResult(stock, "Stok başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<StockDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("notifications/{id}/mark-read")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkNotificationAsRead(int id, [FromBody] MarkNotificationReadDto dto)
    {
        try
        {
            var result = await _stockService.MarkNotificationAsReadAsync(id, dto.UserId, dto.UserName);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Bildirim bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Bildirim okundu olarak işaretlendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _stockService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Stok bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Stok başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
}

public class MarkNotificationReadDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
} 