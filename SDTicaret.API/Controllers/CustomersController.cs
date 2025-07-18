using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerDto>>>> GetAll()
    {
        try
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CustomerDto>>.SuccessResult(customers));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<CustomerDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> GetById(int id)
    {
        try
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<CustomerDto>.ErrorResult("Müşteri bulunamadı"));

            return Ok(ApiResponse<CustomerDto>.SuccessResult(customer));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CustomerDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> Create(CustomerDto dto)
    {
        try
        {
            var customer = await _customerService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, 
                ApiResponse<CustomerDto>.SuccessResult(customer, "Müşteri başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CustomerDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> Update(int id, CustomerDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<CustomerDto>.ErrorResult("ID uyumsuzluğu"));

            var customer = await _customerService.UpdateAsync(dto);
            return Ok(ApiResponse<CustomerDto>.SuccessResult(customer, "Müşteri başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CustomerDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _customerService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Müşteri bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Müşteri başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 