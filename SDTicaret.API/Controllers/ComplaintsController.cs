using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _complaintService;

    public ComplaintsController(IComplaintService complaintService)
    {
        _complaintService = complaintService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ComplaintDto>>>> GetAll()
    {
        try
        {
            var complaints = await _complaintService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ComplaintDto>>.SuccessResult(complaints));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ComplaintDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ComplaintDto>>> GetById(int id)
    {
        try
        {
            var complaint = await _complaintService.GetByIdAsync(id);
            if (complaint == null)
                return NotFound(ApiResponse<ComplaintDto>.ErrorResult("Şikayet bulunamadı"));

            return Ok(ApiResponse<ComplaintDto>.SuccessResult(complaint));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ComplaintDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ComplaintDto>>> Create(ComplaintDto dto)
    {
        try
        {
            var complaint = await _complaintService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = complaint.Id }, 
                ApiResponse<ComplaintDto>.SuccessResult(complaint, "Şikayet başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ComplaintDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ComplaintDto>>> Update(int id, ComplaintDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<ComplaintDto>.ErrorResult("ID uyumsuzluğu"));

            var complaint = await _complaintService.UpdateAsync(dto);
            return Ok(ApiResponse<ComplaintDto>.SuccessResult(complaint, "Şikayet başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ComplaintDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _complaintService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Şikayet bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Şikayet başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 