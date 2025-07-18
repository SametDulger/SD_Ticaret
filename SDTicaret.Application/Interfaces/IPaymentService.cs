using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync();
    Task<PaymentDto?> GetByIdAsync(int id);
    Task<PaymentDto> AddAsync(PaymentDto dto);
    Task<PaymentDto> UpdateAsync(PaymentDto dto);
    Task<bool> DeleteAsync(int id);
} 