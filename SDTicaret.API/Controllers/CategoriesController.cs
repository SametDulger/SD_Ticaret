using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAll()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<CategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(int id)
    {
        try
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<CategoryDto>.ErrorResult("Kategori bulunamadı"));

            return Ok(ApiResponse<CategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(CategoryDto dto)
    {
        try
        {
            var category = await _categoryService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, 
                ApiResponse<CategoryDto>.SuccessResult(category, "Kategori başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(int id, CategoryDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<CategoryDto>.ErrorResult("ID uyumsuzluğu"));

            var category = await _categoryService.UpdateAsync(dto);
            return Ok(ApiResponse<CategoryDto>.SuccessResult(category, "Kategori başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Kategori bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Kategori başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 