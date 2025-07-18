using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;

namespace SDTicaret.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> AddAsync(CategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _unitOfWork.Repository<Category>().AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateAsync(CategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        _unitOfWork.Repository<Category>().Update(category);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        if (category == null) return false;
        
        _unitOfWork.Repository<Category>().Delete(category);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
} 
