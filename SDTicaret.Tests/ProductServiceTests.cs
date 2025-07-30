using SDTicaret.Application.Services;
using SDTicaret.Application.DTOs;
using SDTicaret.Core.Entities;
using Moq;
using AutoMapper;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core.Interfaces;
using Xunit;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_All_Products()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 100, Stock = 10 },
            new Product { Id = 2, Name = "Product 2", Price = 200, Stock = 20 }
        };
        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Price = 100, Stock = 10 },
            new ProductDto { Id = 2, Name = "Product 2", Price = 200, Stock = 20 }
        };
        unitOfWorkMock.Setup(u => u.Repository<Product>().GetAllAsync()).ReturnsAsync(products);
        mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);
        var service = new ProductService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.GetAllAsync();
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Product_When_Exists()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var product = new Product { Id = 1, Name = "Test Product", Price = 100, Stock = 10 };
        var productDto = new ProductDto { Id = 1, Name = "Test Product", Price = 100, Stock = 10 };
        unitOfWorkMock.Setup(u => u.Repository<Product>().GetByIdAsync(1)).ReturnsAsync(product);
        mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);
        var service = new ProductService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.GetByIdAsync(1);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Product_Not_Exists()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        unitOfWorkMock.Setup(u => u.Repository<Product>().GetByIdAsync(999)).ReturnsAsync((Product?)null);
        var service = new ProductService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.GetByIdAsync(999);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Product_Successfully()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var productDto = new ProductDto { Name = "New Product", Price = 150, Stock = 15, CategoryId = 1, SupplierId = 1 };
        var product = new Product { Name = "New Product", Price = 150, Stock = 15, CategoryId = 1, SupplierId = 1 };
        mapperMock.Setup(m => m.Map<Product>(productDto)).Returns(product);
        unitOfWorkMock.Setup(u => u.Repository<Product>().AddAsync(product)).Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);
        var service = new ProductService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.AddAsync(productDto);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        unitOfWorkMock.Verify(u => u.Repository<Product>().AddAsync(product), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Product_Successfully()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var product = new Product { Id = 1, Name = "To Delete" };
        unitOfWorkMock.Setup(u => u.Repository<Product>().GetByIdAsync(1)).ReturnsAsync(product);
        unitOfWorkMock.Setup(u => u.Repository<Product>().Delete(product));
        unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        var service = new ProductService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.DeleteAsync(1);
        // Assert
        Assert.True(result);
        unitOfWorkMock.Verify(u => u.Repository<Product>().Delete(product), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Product_Not_Found()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        unitOfWorkMock.Setup(u => u.Repository<Product>().GetByIdAsync(999)).ReturnsAsync((Product?)null);
        var service = new ProductService(unitOfWorkMock.Object, mapperMock.Object);
        // Act
        var result = await service.DeleteAsync(999);
        // Assert
        Assert.False(result);
    }
} 