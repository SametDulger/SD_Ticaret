using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IContractService
{
    Task<IEnumerable<ContractDto>> GetAllAsync();
    Task<ContractDto?> GetByIdAsync(int id);
    Task<ContractDto> AddAsync(ContractDto dto);
    Task<ContractDto> UpdateAsync(ContractDto dto);
    Task<bool> DeleteAsync(int id);
} 