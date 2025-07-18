using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _unitOfWork.Repository<Employee>().GetAllAsync();
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var employee = await _unitOfWork.Repository<Employee>().GetByIdAsync(id);
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> AddAsync(EmployeeDto dto)
    {
        var employee = _mapper.Map<Employee>(dto);
        await _unitOfWork.Repository<Employee>().AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(EmployeeDto dto)
    {
        var employee = _mapper.Map<Employee>(dto);
        _unitOfWork.Repository<Employee>().Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _unitOfWork.Repository<Employee>().GetByIdAsync(id);
        if (employee == null) return false;
        
        _unitOfWork.Repository<Employee>().Delete(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
