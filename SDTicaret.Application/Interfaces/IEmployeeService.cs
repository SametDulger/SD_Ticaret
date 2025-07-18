using SDTicaret.Application.DTOs;

namespace SDTicaret.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> AddAsync(EmployeeDto dto);
    Task<EmployeeDto> UpdateAsync(EmployeeDto dto);
    Task<bool> DeleteAsync(int id);
} 