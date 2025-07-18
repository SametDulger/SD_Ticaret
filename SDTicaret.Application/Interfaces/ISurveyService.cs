using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface ISurveyService
{
    Task<IEnumerable<SurveyDto>> GetAllAsync();
    Task<SurveyDto?> GetByIdAsync(int id);
    Task<SurveyDto> AddAsync(SurveyDto dto);
    Task<SurveyDto> UpdateAsync(SurveyDto dto);
    Task<bool> DeleteAsync(int id);
} 