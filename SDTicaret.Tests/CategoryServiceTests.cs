using SDTicaret.Application.Services;
using SDTicaret.Application.DTOs;
using SDTicaret.Core.Entities;
using Moq;
using AutoMapper;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core.Interfaces;
using Xunit;

namespace SDTicaret.Tests
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetMainCategoriesAsync_Should_Return_Main_Categories()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var mainCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Elektronik", ParentId = null },
                new Category { Id = 2, Name = "Giyim", ParentId = null }
            };
            
            unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .ReturnsAsync(mainCategories);
            
            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Elektronik", ParentId = null },
                new CategoryDto { Id = 2, Name = "Giyim", ParentId = null }
            };
            
            mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(mainCategories)).Returns(categoryDtos);
            
            var service = new CategoryService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.GetMainCategoriesAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, category => Assert.Null(category.ParentId));
        }

        [Fact]
        public async Task GetSubCategoriesAsync_Should_Return_Sub_Categories()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var subCategories = new List<Category>
            {
                new Category { Id = 3, Name = "Telefon", ParentId = 1 },
                new Category { Id = 4, Name = "Bilgisayar", ParentId = 1 }
            };
            
            unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .ReturnsAsync(subCategories);
            
            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = 3, Name = "Telefon", ParentId = 1 },
                new CategoryDto { Id = 4, Name = "Bilgisayar", ParentId = 1 }
            };
            
            mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(subCategories)).Returns(categoryDtos);
            
            var service = new CategoryService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.GetSubCategoriesAsync(1);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, category => Assert.Equal(1, category.ParentId));
        }

        [Fact]
        public async Task GetCategoryTreeAsync_Should_Return_Hierarchical_Structure()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var allCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Elektronik", ParentId = null },
                new Category { Id = 2, Name = "Giyim", ParentId = null },
                new Category { Id = 3, Name = "Telefon", ParentId = 1 },
                new Category { Id = 4, Name = "Bilgisayar", ParentId = 1 }
            };
            
            unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync()).ReturnsAsync(allCategories);
            unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .ReturnsAsync((System.Linq.Expressions.Expression<System.Func<Category, bool>> predicate) => 
                    allCategories.Where(predicate.Compile()));
            
            var mainCategoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Elektronik", ParentId = null },
                new CategoryDto { Id = 2, Name = "Giyim", ParentId = null }
            };
            
            var subCategoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = 3, Name = "Telefon", ParentId = 1 },
                new CategoryDto { Id = 4, Name = "Bilgisayar", ParentId = 1 }
            };
            
            mapperMock.Setup(m => m.Map<List<CategoryDto>>(It.IsAny<List<Category>>())).Returns(mainCategoryDtos);
            mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(It.IsAny<IEnumerable<Category>>())).Returns(subCategoryDtos);
            
            var service = new CategoryService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.GetCategoryTreeAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_Exception_When_Category_Has_SubCategories()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var category = new Category { Id = 1, Name = "Elektronik" };
            var subCategories = new List<Category> { new Category { Id = 2, Name = "Telefon", ParentId = 1 } };
            
            unitOfWorkMock.Setup(u => u.Repository<Category>().GetByIdAsync(1)).ReturnsAsync(category);
            unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .ReturnsAsync(subCategories);
            
            var service = new CategoryService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_Exception_When_Category_Has_Products()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var category = new Category { Id = 1, Name = "Elektronik" };
            var subCategories = new List<Category>();
            var products = new List<Product> { new Product { Id = 1, Name = "iPhone", CategoryId = 1 } };
            
            unitOfWorkMock.Setup(u => u.Repository<Category>().GetByIdAsync(1)).ReturnsAsync(category);
            unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .ReturnsAsync(subCategories);
            unitOfWorkMock.Setup(u => u.Repository<Product>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Product, bool>>>()))
                .ReturnsAsync(products);
            
            var service = new CategoryService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(1));
        }
    }
} 