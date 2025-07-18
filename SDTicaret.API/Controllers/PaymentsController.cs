using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PaymentDto>>>> GetAll()
    {
        try
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResult(payments));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<PaymentDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> GetById(int id)
    {
        try
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
                return NotFound(ApiResponse<PaymentDto>.ErrorResult("Ödeme bulunamadı"));

            return Ok(ApiResponse<PaymentDto>.SuccessResult(payment));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaymentDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> Create(PaymentDto dto)
    {
        try
        {
            var payment = await _paymentService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, 
                ApiResponse<PaymentDto>.SuccessResult(payment, "Ödeme başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaymentDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> Update(int id, PaymentDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<PaymentDto>.ErrorResult("ID uyumsuzluğu"));

            var payment = await _paymentService.UpdateAsync(dto);
            return Ok(ApiResponse<PaymentDto>.SuccessResult(payment, "Ödeme başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaymentDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _paymentService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Ödeme bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Ödeme başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 