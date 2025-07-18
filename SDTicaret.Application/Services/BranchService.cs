using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class BranchService : IBranchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BranchService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BranchDto>> GetAllAsync()
    {
        var branches = await _unitOfWork.Repository<Branch>().GetAllAsync();
        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<BranchDto?> GetByIdAsync(int id)
    {
        var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(id);
        return _mapper.Map<BranchDto>(branch);
    }

    public async Task<BranchDto> AddAsync(BranchDto dto)
    {
        var branch = _mapper.Map<Branch>(dto);
        await _unitOfWork.Repository<Branch>().AddAsync(branch);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BranchDto>(branch);
    }

    public async Task<BranchDto> UpdateAsync(BranchDto dto)
    {
        var branch = _mapper.Map<Branch>(dto);
        _unitOfWork.Repository<Branch>().Update(branch);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BranchDto>(branch);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(id);
        if (branch == null) return false;
        
        _unitOfWork.Repository<Branch>().Delete(branch);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
