using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SupplierService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync()
    {
        var suppliers = await _unitOfWork.Repository<Supplier>().GetAllAsync();
        return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var supplier = await _unitOfWork.Repository<Supplier>().GetByIdAsync(id);
        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task<SupplierDto> AddAsync(SupplierDto dto)
    {
        var supplier = _mapper.Map<Supplier>(dto);
        await _unitOfWork.Repository<Supplier>().AddAsync(supplier);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task<SupplierDto> UpdateAsync(SupplierDto dto)
    {
        var supplier = _mapper.Map<Supplier>(dto);
        _unitOfWork.Repository<Supplier>().Update(supplier);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SupplierDto>(supplier);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _unitOfWork.Repository<Supplier>().GetByIdAsync(id);
        if (supplier == null) return false;
        
        _unitOfWork.Repository<Supplier>().Delete(supplier);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
