using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class ComplaintService : IComplaintService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ComplaintService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ComplaintDto>> GetAllAsync()
    {
        var complaints = await _unitOfWork.Repository<Complaint>().GetAllAsync();
        return _mapper.Map<IEnumerable<ComplaintDto>>(complaints);
    }

    public async Task<ComplaintDto?> GetByIdAsync(int id)
    {
        var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(id);
        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task<ComplaintDto> AddAsync(ComplaintDto dto)
    {
        var complaint = _mapper.Map<Complaint>(dto);
        await _unitOfWork.Repository<Complaint>().AddAsync(complaint);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task<ComplaintDto> UpdateAsync(ComplaintDto dto)
    {
        var complaint = _mapper.Map<Complaint>(dto);
        _unitOfWork.Repository<Complaint>().Update(complaint);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(id);
        if (complaint == null) return false;
        
        _unitOfWork.Repository<Complaint>().Delete(complaint);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
