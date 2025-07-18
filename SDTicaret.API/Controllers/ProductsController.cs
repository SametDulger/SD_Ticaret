using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResult(products));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(ApiResponse<ProductDto>.ErrorResult("Ürün bulunamadı"));

            return Ok(ApiResponse<ProductDto>.SuccessResult(product));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(ProductDto dto)
    {
        try
        {
            var product = await _productService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, 
                ApiResponse<ProductDto>.SuccessResult(product, "Ürün başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, ProductDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<ProductDto>.ErrorResult("ID uyumsuzluğu"));

            var product = await _productService.UpdateAsync(dto);
            return Ok(ApiResponse<ProductDto>.SuccessResult(product, "Ürün başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Ürün bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Ürün başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 