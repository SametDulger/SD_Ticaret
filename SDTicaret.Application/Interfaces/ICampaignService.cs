using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface ICampaignService
{
    Task<IEnumerable<CampaignDto>> GetAllAsync();
    Task<CampaignDto?> GetByIdAsync(int id);
    Task<CampaignDto> AddAsync(CampaignDto dto);
    Task<CampaignDto> UpdateAsync(CampaignDto dto);
    Task<bool> DeleteAsync(int id);
} 