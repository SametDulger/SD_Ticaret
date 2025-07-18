using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> AddAsync(ProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        await _unitOfWork.Repository<Product>().AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateAsync(ProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        _unitOfWork.Repository<Product>().Update(product);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null) return false;
        
        _unitOfWork.Repository<Product>().Delete(product);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
