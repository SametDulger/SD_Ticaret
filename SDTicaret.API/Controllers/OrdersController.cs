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

    [HttpGet("status/{status}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetByStatus(string status)
    {
        try
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetByCustomer(int customerId)
    {
        try
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("pending")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetPendingOrders()
    {
        try
        {
            var orders = await _orderService.GetPendingOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("processing")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetProcessingOrders()
    {
        try
        {
            var orders = await _orderService.GetProcessingOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orders));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("shipped")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetShippedOrders()
    {
        try
        {
            var orders = await _orderService.GetShippedOrdersAsync();
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

    [HttpGet("{id}/history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderStatusHistoryDto>>>> GetStatusHistory(int id)
    {
        try
        {
            var history = await _orderService.GetOrderStatusHistoryAsync(id);
            return Ok(ApiResponse<IEnumerable<OrderStatusHistoryDto>>.SuccessResult(history));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<OrderStatusHistoryDto>>.ErrorResult(ex.Message));
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

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder(CreateOrderDto dto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(dto);
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

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        try
        {
            if (id != dto.OrderId)
                return BadRequest(ApiResponse<OrderDto>.ErrorResult("ID uyumsuzluğu"));

            var order = await _orderService.UpdateOrderStatusAsync(dto);
            return Ok(ApiResponse<OrderDto>.SuccessResult(order, "Sipariş durumu başarıyla güncellendi"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<OrderDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(int id, [FromBody] CancelOrderDto dto)
    {
        try
        {
            var result = await _orderService.CancelOrderAsync(id, dto.Reason, dto.UserId, dto.UserName);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Sipariş bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Sipariş başarıyla iptal edildi"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
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

public class CancelOrderDto
{
    public string Reason { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? UserName { get; set; }
} 