using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IBranchService
{
    Task<IEnumerable<BranchDto>> GetAllAsync();
    Task<BranchDto?> GetByIdAsync(int id);
    Task<BranchDto> AddAsync(BranchDto dto);
    Task<BranchDto> UpdateAsync(BranchDto dto);
    Task<bool> DeleteAsync(int id);
} 