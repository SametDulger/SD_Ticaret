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
    public class StockServiceTests
    {
        [Fact]
        public async Task StockInAsync_Should_Increase_Stock_Quantity()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var stock = new Stock { Id = 1, ProductId = 1, BranchId = 1, Quantity = 10, MinimumStock = 5, MaximumStock = 100 };
            var stockDto = new StockDto { Id = 1, ProductId = 1, BranchId = 1, Quantity = 15, MinimumStock = 5, MaximumStock = 100 };
            
            unitOfWorkMock.Setup(u => u.Repository<Stock>().GetByIdAsync(1)).ReturnsAsync(stock);
            unitOfWorkMock.Setup(u => u.Repository<Stock>().Update(It.IsAny<Stock>())).Verifiable();
            unitOfWorkMock.Setup(u => u.Repository<StockMovement>().AddAsync(It.IsAny<StockMovement>())).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.Repository<StockNotification>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<StockNotification, bool>>>()))
                .ReturnsAsync(new List<StockNotification>());
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            mapperMock.Setup(m => m.Map<StockDto>(stock)).Returns(stockDto);
            
            var service = new StockService(unitOfWorkMock.Object, mapperMock.Object);
            
            var stockInDto = new StockInDto { StockId = 1, Quantity = 5, Reason = "Test" };
            
            // Act
            var result = await service.StockInAsync(stockInDto);
            
            // Assert
            Assert.Equal(15, result.Quantity);
            unitOfWorkMock.Verify(u => u.Repository<Stock>().Update(It.IsAny<Stock>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Repository<StockMovement>().AddAsync(It.IsAny<StockMovement>()), Times.Once);
        }

        [Fact]
        public async Task StockOutAsync_Should_Decrease_Stock_Quantity()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var stock = new Stock { Id = 1, ProductId = 1, BranchId = 1, Quantity = 10, MinimumStock = 5, MaximumStock = 100 };
            var stockDto = new StockDto { Id = 1, ProductId = 1, BranchId = 1, Quantity = 5, MinimumStock = 5, MaximumStock = 100 };
            
            unitOfWorkMock.Setup(u => u.Repository<Stock>().GetByIdAsync(1)).ReturnsAsync(stock);
            unitOfWorkMock.Setup(u => u.Repository<Stock>().Update(It.IsAny<Stock>())).Verifiable();
            unitOfWorkMock.Setup(u => u.Repository<StockMovement>().AddAsync(It.IsAny<StockMovement>())).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.Repository<StockNotification>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<StockNotification, bool>>>()))
                .ReturnsAsync(new List<StockNotification>());
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            mapperMock.Setup(m => m.Map<StockDto>(stock)).Returns(stockDto);
            
            var service = new StockService(unitOfWorkMock.Object, mapperMock.Object);
            
            var stockOutDto = new StockOutDto { StockId = 1, Quantity = 5, Reason = "Test" };
            
            // Act
            var result = await service.StockOutAsync(stockOutDto);
            
            // Assert
            Assert.Equal(5, result.Quantity);
            unitOfWorkMock.Verify(u => u.Repository<Stock>().Update(It.IsAny<Stock>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Repository<StockMovement>().AddAsync(It.IsAny<StockMovement>()), Times.Once);
        }

        [Fact]
        public async Task StockOutAsync_Should_Throw_Exception_When_Insufficient_Stock()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var stock = new Stock { Id = 1, ProductId = 1, BranchId = 1, Quantity = 5, MinimumStock = 5, MaximumStock = 100 };
            
            unitOfWorkMock.Setup(u => u.Repository<Stock>().GetByIdAsync(1)).ReturnsAsync(stock);
            
            var service = new StockService(unitOfWorkMock.Object, mapperMock.Object);
            
            var stockOutDto = new StockOutDto { StockId = 1, Quantity = 10, Reason = "Test" };
            
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.StockOutAsync(stockOutDto));
        }

        [Fact]
        public async Task GetLowStockItemsAsync_Should_Return_Low_Stock_Items()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var lowStockItems = new List<Stock>
            {
                new Stock { Id = 1, ProductId = 1, BranchId = 1, Quantity = 3, MinimumStock = 5, IsLowStockAlertEnabled = true },
                new Stock { Id = 2, ProductId = 2, BranchId = 1, Quantity = 0, MinimumStock = 5, IsLowStockAlertEnabled = true }
            };
            
            var stockDtos = new List<StockDto>
            {
                new StockDto { Id = 1, ProductId = 1, BranchId = 1, Quantity = 3, MinimumStock = 5, IsLowStock = true },
                new StockDto { Id = 2, ProductId = 2, BranchId = 1, Quantity = 0, MinimumStock = 5, IsOutOfStock = true }
            };
            
            unitOfWorkMock.Setup(u => u.Repository<Stock>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Stock, bool>>>()))
                .ReturnsAsync(lowStockItems);
            
            mapperMock.Setup(m => m.Map<IEnumerable<StockDto>>(lowStockItems)).Returns(stockDtos);
            
            var service = new StockService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.GetLowStockItemsAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, item => item.IsLowStock);
        }

        [Fact]
        public async Task GetOutOfStockItemsAsync_Should_Return_Out_Of_Stock_Items()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var outOfStockItems = new List<Stock>
            {
                new Stock { Id = 1, ProductId = 1, BranchId = 1, Quantity = 0, MinimumStock = 5 },
                new Stock { Id = 2, ProductId = 2, BranchId = 1, Quantity = 0, MinimumStock = 5 }
            };
            
            var stockDtos = new List<StockDto>
            {
                new StockDto { Id = 1, ProductId = 1, BranchId = 1, Quantity = 0, MinimumStock = 5, IsOutOfStock = true },
                new StockDto { Id = 2, ProductId = 2, BranchId = 1, Quantity = 0, MinimumStock = 5, IsOutOfStock = true }
            };
            
            unitOfWorkMock.Setup(u => u.Repository<Stock>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Stock, bool>>>()))
                .ReturnsAsync(outOfStockItems);
            
            mapperMock.Setup(m => m.Map<IEnumerable<StockDto>>(outOfStockItems)).Returns(stockDtos);
            
            var service = new StockService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.GetOutOfStockItemsAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.True(item.IsOutOfStock));
        }

        [Fact]
        public async Task MarkNotificationAsReadAsync_Should_Mark_Notification_As_Read()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var notification = new StockNotification { Id = 1, StockId = 1, IsRead = false };
            
            unitOfWorkMock.Setup(u => u.Repository<StockNotification>().GetByIdAsync(1)).ReturnsAsync(notification);
            unitOfWorkMock.Setup(u => u.Repository<StockNotification>().Update(It.IsAny<StockNotification>())).Verifiable();
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            var service = new StockService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.MarkNotificationAsReadAsync(1, 1, "TestUser");
            
            // Assert
            Assert.True(result);
            unitOfWorkMock.Verify(u => u.Repository<StockNotification>().Update(It.IsAny<StockNotification>()), Times.Once);
        }
    }
} 