using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SupplierDto>>>> GetAll()
    {
        try
        {
            var suppliers = await _supplierService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<SupplierDto>>.SuccessResult(suppliers));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<SupplierDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SupplierDto>>> GetById(int id)
    {
        try
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound(ApiResponse<SupplierDto>.ErrorResult("Tedarikçi bulunamadı"));

            return Ok(ApiResponse<SupplierDto>.SuccessResult(supplier));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SupplierDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SupplierDto>>> Create(SupplierDto dto)
    {
        try
        {
            var supplier = await _supplierService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, 
                ApiResponse<SupplierDto>.SuccessResult(supplier, "Tedarikçi başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SupplierDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<SupplierDto>>> Update(int id, SupplierDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<SupplierDto>.ErrorResult("ID uyumsuzluğu"));

            var supplier = await _supplierService.UpdateAsync(dto);
            return Ok(ApiResponse<SupplierDto>.SuccessResult(supplier, "Tedarikçi başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SupplierDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _supplierService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Tedarikçi bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Tedarikçi başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 