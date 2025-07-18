using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class CampaignService : ICampaignService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CampaignService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CampaignDto>> GetAllAsync()
    {
        var campaigns = await _unitOfWork.Repository<Campaign>().GetAllAsync();
        return _mapper.Map<IEnumerable<CampaignDto>>(campaigns);
    }

    public async Task<CampaignDto?> GetByIdAsync(int id)
    {
        var campaign = await _unitOfWork.Repository<Campaign>().GetByIdAsync(id);
        return _mapper.Map<CampaignDto>(campaign);
    }

    public async Task<CampaignDto> AddAsync(CampaignDto dto)
    {
        var campaign = _mapper.Map<Campaign>(dto);
        await _unitOfWork.Repository<Campaign>().AddAsync(campaign);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CampaignDto>(campaign);
    }

    public async Task<CampaignDto> UpdateAsync(CampaignDto dto)
    {
        var campaign = _mapper.Map<Campaign>(dto);
        _unitOfWork.Repository<Campaign>().Update(campaign);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CampaignDto>(campaign);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var campaign = await _unitOfWork.Repository<Campaign>().GetByIdAsync(id);
        if (campaign == null) return false;
        
        _unitOfWork.Repository<Campaign>().Delete(campaign);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
