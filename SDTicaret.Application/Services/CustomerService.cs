using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(id);
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> AddAsync(CustomerDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        await _unitOfWork.Repository<Customer>().AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> UpdateAsync(CustomerDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        _unitOfWork.Repository<Customer>().Update(customer);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(id);
        if (customer == null) return false;
        
        _unitOfWork.Repository<Customer>().Delete(customer);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
