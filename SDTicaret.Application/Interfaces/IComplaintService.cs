using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IComplaintService
{
    Task<IEnumerable<ComplaintDto>> GetAllAsync();
    Task<ComplaintDto?> GetByIdAsync(int id);
    Task<ComplaintDto> AddAsync(ComplaintDto dto);
    Task<ComplaintDto> UpdateAsync(ComplaintDto dto);
    Task<bool> DeleteAsync(int id);
} 