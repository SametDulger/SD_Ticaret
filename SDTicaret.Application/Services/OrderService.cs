using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> AddAsync(OrderDto dto)
    {
        var order = _mapper.Map<Order>(dto);
        await _unitOfWork.Repository<Order>().AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> UpdateAsync(OrderDto dto)
    {
        var order = _mapper.Map<Order>(dto);
        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
        if (order == null) return false;
        
        _unitOfWork.Repository<Order>().Delete(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
