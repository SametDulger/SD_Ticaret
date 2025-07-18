using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderItemService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
    {
        var orderItems = await _unitOfWork.Repository<OrderItem>().GetAllAsync();
        return _mapper.Map<IEnumerable<OrderItemDto>>(orderItems);
    }

    public async Task<OrderItemDto?> GetByIdAsync(int id)
    {
        var orderItem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(id);
        return _mapper.Map<OrderItemDto>(orderItem);
    }

    public async Task<OrderItemDto> AddAsync(OrderItemDto dto)
    {
        var orderItem = _mapper.Map<OrderItem>(dto);
        await _unitOfWork.Repository<OrderItem>().AddAsync(orderItem);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OrderItemDto>(orderItem);
    }

    public async Task<OrderItemDto> UpdateAsync(OrderItemDto dto)
    {
        var orderItem = _mapper.Map<OrderItem>(dto);
        _unitOfWork.Repository<OrderItem>().Update(orderItem);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OrderItemDto>(orderItem);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var orderItem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(id);
        if (orderItem == null) return false;
        
        _unitOfWork.Repository<OrderItem>().Delete(orderItem);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
