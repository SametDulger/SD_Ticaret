using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class StockService : IStockService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StockService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StockDto>> GetAllAsync()
    {
        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync();
        return _mapper.Map<IEnumerable<StockDto>>(stocks);
    }

    public async Task<StockDto?> GetByIdAsync(int id)
    {
        var stock = await _unitOfWork.Repository<Stock>().GetByIdAsync(id);
        return _mapper.Map<StockDto>(stock);
    }

    public async Task<StockDto> AddAsync(StockDto dto)
    {
        var stock = _mapper.Map<Stock>(dto);
        await _unitOfWork.Repository<Stock>().AddAsync(stock);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StockDto>(stock);
    }

    public async Task<StockDto> UpdateAsync(StockDto dto)
    {
        var stock = _mapper.Map<Stock>(dto);
        _unitOfWork.Repository<Stock>().Update(stock);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StockDto>(stock);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var stock = await _unitOfWork.Repository<Stock>().GetByIdAsync(id);
        if (stock == null) return false;
        
        _unitOfWork.Repository<Stock>().Delete(stock);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
