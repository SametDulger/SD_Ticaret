using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync()
    {
        var payments = await _unitOfWork.Repository<Payment>().GetAllAsync();
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(id);
        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto> AddAsync(PaymentDto dto)
    {
        var payment = _mapper.Map<Payment>(dto);
        await _unitOfWork.Repository<Payment>().AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto> UpdateAsync(PaymentDto dto)
    {
        var payment = _mapper.Map<Payment>(dto);
        _unitOfWork.Repository<Payment>().Update(payment);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(id);
        if (payment == null) return false;
        
        _unitOfWork.Repository<Payment>().Delete(payment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
