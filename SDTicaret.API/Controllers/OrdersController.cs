using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetAll()
    {
        try
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(int id)
    {
        try
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<OrderDto>.ErrorResult("Sipariş bulunamadı"));

            return Ok(ApiResponse<OrderDto>.SuccessResult(order));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> Create(OrderDto dto)
    {
        try
        {
            var order = await _orderService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, 
                ApiResponse<OrderDto>.SuccessResult(order, "Sipariş başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> Update(int id, OrderDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<OrderDto>.ErrorResult("ID uyumsuzluğu"));

            var order = await _orderService.UpdateAsync(dto);
            return Ok(ApiResponse<OrderDto>.SuccessResult(order, "Sipariş başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _orderService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Sipariş bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Sipariş başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 