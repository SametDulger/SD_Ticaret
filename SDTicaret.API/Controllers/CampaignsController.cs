using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;

    public CampaignsController(ICampaignService campaignService)
    {
        _campaignService = campaignService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CampaignDto>>>> GetAll()
    {
        try
        {
            var campaigns = await _campaignService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CampaignDto>>.SuccessResult(campaigns));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<CampaignDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CampaignDto>>> GetById(int id)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            if (campaign == null)
                return NotFound(ApiResponse<CampaignDto>.ErrorResult("Kampanya bulunamadı"));

            return Ok(ApiResponse<CampaignDto>.SuccessResult(campaign));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CampaignDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CampaignDto>>> Create(CampaignDto dto)
    {
        try
        {
            var campaign = await _campaignService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = campaign.Id }, 
                ApiResponse<CampaignDto>.SuccessResult(campaign, "Kampanya başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CampaignDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CampaignDto>>> Update(int id, CampaignDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<CampaignDto>.ErrorResult("ID uyumsuzluğu"));

            var campaign = await _campaignService.UpdateAsync(dto);
            return Ok(ApiResponse<CampaignDto>.SuccessResult(campaign, "Kampanya başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CampaignDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _campaignService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Kampanya bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Kampanya başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 