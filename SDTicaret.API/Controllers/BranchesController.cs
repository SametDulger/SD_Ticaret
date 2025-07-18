using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly IBranchService _branchService;

    public BranchesController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<BranchDto>>>> GetAll()
    {
        try
        {
            var branches = await _branchService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResult(branches));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<BranchDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BranchDto>>> GetById(int id)
    {
        try
        {
            var branch = await _branchService.GetByIdAsync(id);
            if (branch == null)
                return NotFound(ApiResponse<BranchDto>.ErrorResult("Şube bulunamadı"));

            return Ok(ApiResponse<BranchDto>.SuccessResult(branch));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BranchDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BranchDto>>> Create(BranchDto dto)
    {
        try
        {
            var branch = await _branchService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = branch.Id }, 
                ApiResponse<BranchDto>.SuccessResult(branch, "Şube başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BranchDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<BranchDto>>> Update(int id, BranchDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<BranchDto>.ErrorResult("ID uyumsuzluğu"));

            var branch = await _branchService.UpdateAsync(dto);
            return Ok(ApiResponse<BranchDto>.SuccessResult(branch, "Şube başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BranchDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _branchService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Şube bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Şube başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 