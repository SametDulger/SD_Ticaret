using Microsoft.AspNetCore.Mvc;
using SDTicaret.API.Models;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurveysController : ControllerBase
{
    private readonly ISurveyService _surveyService;

    public SurveysController(ISurveyService surveyService)
    {
        _surveyService = surveyService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SurveyDto>>>> GetAll()
    {
        try
        {
            var surveys = await _surveyService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<SurveyDto>>.SuccessResult(surveys));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<SurveyDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SurveyDto>>> GetById(int id)
    {
        try
        {
            var survey = await _surveyService.GetByIdAsync(id);
            if (survey == null)
                return NotFound(ApiResponse<SurveyDto>.ErrorResult("Anket bulunamadı"));

            return Ok(ApiResponse<SurveyDto>.SuccessResult(survey));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SurveyDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SurveyDto>>> Create(SurveyDto dto)
    {
        try
        {
            var survey = await _surveyService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = survey.Id }, 
                ApiResponse<SurveyDto>.SuccessResult(survey, "Anket başarıyla oluşturuldu"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SurveyDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<SurveyDto>>> Update(int id, SurveyDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<SurveyDto>.ErrorResult("ID uyumsuzluğu"));

            var survey = await _surveyService.UpdateAsync(dto);
            return Ok(ApiResponse<SurveyDto>.SuccessResult(survey, "Anket başarıyla güncellendi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<SurveyDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _surveyService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("Anket bulunamadı"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "Anket başarıyla silindi"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }
} 