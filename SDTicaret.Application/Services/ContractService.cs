using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class ContractService : IContractService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ContractService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContractDto>> GetAllAsync()
    {
        var contracts = await _unitOfWork.Repository<Contract>().GetAllAsync();
        return _mapper.Map<IEnumerable<ContractDto>>(contracts);
    }

    public async Task<ContractDto?> GetByIdAsync(int id)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetByIdAsync(id);
        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> AddAsync(ContractDto dto)
    {
        var contract = _mapper.Map<Contract>(dto);
        await _unitOfWork.Repository<Contract>().AddAsync(contract);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<ContractDto> UpdateAsync(ContractDto dto)
    {
        var contract = _mapper.Map<Contract>(dto);
        _unitOfWork.Repository<Contract>().Update(contract);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ContractDto>(contract);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var contract = await _unitOfWork.Repository<Contract>().GetByIdAsync(id);
        if (contract == null) return false;
        
        _unitOfWork.Repository<Contract>().Delete(contract);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
