using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemsController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderItemDto>>>> GetAll()
    {
        try
        {
            var orderItems = await _orderItemService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<OrderItemDto>>.SuccessResult(orderItems));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderItemDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderItemDto>>> GetById(int id)
    {
        try
        {
            var orderItem = await _orderItemService.GetByIdAsync(id);
            if (orderItem == null)
                return NotFound(ApiResponse<OrderItemDto>.ErrorResult("Sipariş kalemi bulunamadı"));

            return Ok(ApiResponse<OrderItemDto>.SuccessResult(orderItem));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderItemDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderItemDto>>> Create(OrderItemDto dto)
    {
        try
        {
            var orderItem = await _orderItemService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = orderItem.Id }, 
                ApiResponse<OrderItemDto>.SuccessResult(orderItem, "Sipariş kalemi başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderItemDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<OrderItemDto>>> Update(int id, OrderItemDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<OrderItemDto>.ErrorResult("ID uyumsuzluğu"));

            var orderItem = await _orderItemService.UpdateAsync(dto);
            return Ok(ApiResponse<OrderItemDto>.SuccessResult(orderItem, "Sipariş kalemi başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderItemDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _orderItemService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Sipariş kalemi bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Sipariş kalemi başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 