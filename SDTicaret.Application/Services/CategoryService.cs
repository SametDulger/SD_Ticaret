using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;
using System.Linq.Expressions;

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
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(dto.Id);
        if (category == null)
            throw new InvalidOperationException("Kategori bulunamadı");

        _mapper.Map(dto, category);
        _unitOfWork.Repository<Category>().Update(category);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        if (category == null) return false;
        
        // Alt kategorileri kontrol et
        var subCategories = await _unitOfWork.Repository<Category>().GetAllAsync(c => c.ParentId == id);
        if (subCategories.Any())
            throw new InvalidOperationException("Bu kategorinin alt kategorileri var. Önce alt kategorileri silin.");
        
        // Ürünleri kontrol et
        var products = await _unitOfWork.Repository<Product>().GetAllAsync(p => p.CategoryId == id);
        if (products.Any())
            throw new InvalidOperationException("Bu kategoride ürünler var. Önce ürünleri silin.");
        
        _unitOfWork.Repository<Category>().Delete(category);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CategoryDto>> GetMainCategoriesAsync()
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync(c => c.ParentId == null);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId)
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync(c => c.ParentId == parentId);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoryTreeAsync()
    {
        var allCategories = await _unitOfWork.Repository<Category>().GetAllAsync();
        var mainCategories = allCategories.Where(c => c.ParentId == null).ToList();
        
        var categoryDtos = _mapper.Map<List<CategoryDto>>(mainCategories);
        
        foreach (var categoryDto in categoryDtos)
        {
            categoryDto.SubCategories = (await GetSubCategoriesAsync(categoryDto.Id)).ToList();
        }
        
        return categoryDtos;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync(p => p.CategoryId == categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
} 
