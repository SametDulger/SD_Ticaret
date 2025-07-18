using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class SurveyService : ISurveyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SurveyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SurveyDto>> GetAllAsync()
    {
        var surveys = await _unitOfWork.Repository<Survey>().GetAllAsync();
        return _mapper.Map<IEnumerable<SurveyDto>>(surveys);
    }

    public async Task<SurveyDto?> GetByIdAsync(int id)
    {
        var survey = await _unitOfWork.Repository<Survey>().GetByIdAsync(id);
        return _mapper.Map<SurveyDto>(survey);
    }

    public async Task<SurveyDto> AddAsync(SurveyDto dto)
    {
        var survey = _mapper.Map<Survey>(dto);
        await _unitOfWork.Repository<Survey>().AddAsync(survey);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SurveyDto>(survey);
    }

    public async Task<SurveyDto> UpdateAsync(SurveyDto dto)
    {
        var survey = _mapper.Map<Survey>(dto);
        _unitOfWork.Repository<Survey>().Update(survey);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SurveyDto>(survey);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var survey = await _unitOfWork.Repository<Survey>().GetByIdAsync(id);
        if (survey == null) return false;
        
        _unitOfWork.Repository<Survey>().Delete(survey);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
