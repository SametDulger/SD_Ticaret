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