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
    public class OrderServiceTests
    {
        [Fact]
        public async Task CreateOrderAsync_Should_Create_Order_With_Items()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var createOrderDto = new CreateOrderDto
            {
                CustomerId = 1,
                ShippingAddress = "Test Address",
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = 1, Quantity = 2, UnitPrice = 10.0m },
                    new CreateOrderItemDto { ProductId = 2, Quantity = 1, UnitPrice = 15.0m }
                }
            };
            
            var order = new Order { Id = 1, CustomerId = 1, OrderNumber = "ORD-20241201-12345678" };
            var orderDto = new OrderDto { Id = 1, CustomerId = 1, OrderNumber = "ORD-20241201-12345678" };
            
            unitOfWorkMock.Setup(u => u.Repository<Order>().AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.Repository<OrderItem>().AddAsync(It.IsAny<OrderItem>())).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.Repository<OrderStatusHistory>().AddAsync(It.IsAny<OrderStatusHistory>())).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDto);
            
            var service = new OrderService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.CreateOrderAsync(createOrderDto);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            unitOfWorkMock.Verify(u => u.Repository<Order>().AddAsync(It.IsAny<Order>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Repository<OrderItem>().AddAsync(It.IsAny<OrderItem>()), Times.Exactly(2));
            unitOfWorkMock.Verify(u => u.Repository<OrderStatusHistory>().AddAsync(It.IsAny<OrderStatusHistory>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_Update_Status_And_Create_History()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var order = new Order { Id = 1, OrderStatus = "Pending" };
            var orderDto = new OrderDto { Id = 1, OrderStatus = "Confirmed" };
            
            var updateDto = new UpdateOrderStatusDto
            {
                OrderId = 1,
                NewStatus = "Confirmed",
                Notes = "Sipariş onaylandı",
                ChangedByUserId = 1,
                ChangedByUserName = "TestUser"
            };
            
            unitOfWorkMock.Setup(u => u.Repository<Order>().GetByIdAsync(1)).ReturnsAsync(order);
            unitOfWorkMock.Setup(u => u.Repository<Order>().Update(It.IsAny<Order>())).Verifiable();
            unitOfWorkMock.Setup(u => u.Repository<OrderStatusHistory>().AddAsync(It.IsAny<OrderStatusHistory>())).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.Repository<OrderItem>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderItem, bool>>>()))
                .ReturnsAsync(new List<OrderItem>());
            unitOfWorkMock.Setup(u => u.Repository<OrderStatusHistory>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderStatusHistory, bool>>>()))
                .ReturnsAsync(new List<OrderStatusHistory>());
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDto);
            mapperMock.Setup(m => m.Map<ICollection<OrderItemDto>>(It.IsAny<IEnumerable<OrderItem>>())).Returns(new List<OrderItemDto>());
            mapperMock.Setup(m => m.Map<ICollection<OrderStatusHistoryDto>>(It.IsAny<IEnumerable<OrderStatusHistory>>())).Returns(new List<OrderStatusHistoryDto>());
            
            var service = new OrderService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.UpdateOrderStatusAsync(updateDto);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Confirmed", result.OrderStatus);
            unitOfWorkMock.Verify(u => u.Repository<Order>().Update(It.IsAny<Order>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Repository<OrderStatusHistory>().AddAsync(It.IsAny<OrderStatusHistory>()), Times.Once);
        }

        [Fact]
        public async Task CancelOrderAsync_Should_Cancel_Order_When_Not_Delivered()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var order = new Order { Id = 1, OrderStatus = "Pending" };
            var orderDto = new OrderDto { Id = 1, OrderStatus = "Cancelled" };
            
            unitOfWorkMock.Setup(u => u.Repository<Order>().GetByIdAsync(1)).ReturnsAsync(order);
            unitOfWorkMock.Setup(u => u.Repository<Order>().Update(It.IsAny<Order>())).Verifiable();
            unitOfWorkMock.Setup(u => u.Repository<OrderStatusHistory>().AddAsync(It.IsAny<OrderStatusHistory>())).Returns(Task.CompletedTask);
            unitOfWorkMock.Setup(u => u.Repository<OrderItem>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderItem, bool>>>()))
                .ReturnsAsync(new List<OrderItem>());
            unitOfWorkMock.Setup(u => u.Repository<OrderStatusHistory>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderStatusHistory, bool>>>()))
                .ReturnsAsync(new List<OrderStatusHistory>());
            unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            
            mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDto);
            mapperMock.Setup(m => m.Map<ICollection<OrderItemDto>>(It.IsAny<IEnumerable<OrderItem>>())).Returns(new List<OrderItemDto>());
            mapperMock.Setup(m => m.Map<ICollection<OrderStatusHistoryDto>>(It.IsAny<IEnumerable<OrderStatusHistory>>())).Returns(new List<OrderStatusHistoryDto>());
            
            var service = new OrderService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.CancelOrderAsync(1, "Müşteri iptal etti", 1, "TestUser");
            
            // Assert
            Assert.True(result);
            unitOfWorkMock.Verify(u => u.Repository<Order>().Update(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task CancelOrderAsync_Should_Throw_Exception_When_Order_Delivered()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var order = new Order { Id = 1, OrderStatus = "Delivered" };
            
            unitOfWorkMock.Setup(u => u.Repository<Order>().GetByIdAsync(1)).ReturnsAsync(order);
            
            var service = new OrderService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                service.CancelOrderAsync(1, "Test reason"));
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_Should_Return_Orders_With_Status()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var orders = new List<Order>
            {
                new Order { Id = 1, OrderStatus = "Pending" },
                new Order { Id = 2, OrderStatus = "Pending" }
            };
            
            var orderDtos = new List<OrderDto>
            {
                new OrderDto { Id = 1, OrderStatus = "Pending" },
                new OrderDto { Id = 2, OrderStatus = "Pending" }
            };
            
            unitOfWorkMock.Setup(u => u.Repository<Order>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Order, bool>>>()))
                .ReturnsAsync(orders);
            unitOfWorkMock.Setup(u => u.Repository<OrderItem>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderItem, bool>>>()))
                .ReturnsAsync(new List<OrderItem>());
            unitOfWorkMock.Setup(u => u.Repository<OrderStatusHistory>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderStatusHistory, bool>>>()))
                .ReturnsAsync(new List<OrderStatusHistory>());
            
            mapperMock.Setup(m => m.Map<IEnumerable<OrderDto>>(orders)).Returns(orderDtos);
            mapperMock.Setup(m => m.Map<ICollection<OrderItemDto>>(It.IsAny<IEnumerable<OrderItem>>())).Returns(new List<OrderItemDto>());
            mapperMock.Setup(m => m.Map<ICollection<OrderStatusHistoryDto>>(It.IsAny<IEnumerable<OrderStatusHistory>>())).Returns(new List<OrderStatusHistoryDto>());
            
            var service = new OrderService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.GetOrdersByStatusAsync("Pending");
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, order => Assert.Equal("Pending", order.OrderStatus));
        }

        [Fact]
        public async Task GetOrderStatusHistoryAsync_Should_Return_Status_History()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var history = new List<OrderStatusHistory>
            {
                new OrderStatusHistory { Id = 1, OrderId = 1, PreviousStatus = "", NewStatus = "Pending" },
                new OrderStatusHistory { Id = 2, OrderId = 1, PreviousStatus = "Pending", NewStatus = "Confirmed" }
            };
            
            var historyDtos = new List<OrderStatusHistoryDto>
            {
                new OrderStatusHistoryDto { Id = 1, OrderId = 1, PreviousStatus = "", NewStatus = "Pending" },
                new OrderStatusHistoryDto { Id = 2, OrderId = 1, PreviousStatus = "Pending", NewStatus = "Confirmed" }
            };
            
            unitOfWorkMock.Setup(u => u.Repository<OrderStatusHistory>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderStatusHistory, bool>>>()))
                .ReturnsAsync(history);
            
            mapperMock.Setup(m => m.Map<IEnumerable<OrderStatusHistoryDto>>(history)).Returns(historyDtos);
            
            var service = new OrderService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.GetOrderStatusHistoryAsync(1);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, h => h.NewStatus == "Pending");
            Assert.Contains(result, h => h.NewStatus == "Confirmed");
        }

        [Fact]
        public async Task GetPendingOrdersAsync_Should_Return_Pending_Orders()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperMock = new Mock<IMapper>();
            
            var orders = new List<Order>
            {
                new Order { Id = 1, OrderStatus = "Pending" },
                new Order { Id = 2, OrderStatus = "Pending" }
            };
            
            var orderDtos = new List<OrderDto>
            {
                new OrderDto { Id = 1, OrderStatus = "Pending" },
                new OrderDto { Id = 2, OrderStatus = "Pending" }
            };
            
            unitOfWorkMock.Setup(u => u.Repository<Order>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Order, bool>>>()))
                .ReturnsAsync(orders);
            unitOfWorkMock.Setup(u => u.Repository<OrderItem>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderItem, bool>>>()))
                .ReturnsAsync(new List<OrderItem>());
            unitOfWorkMock.Setup(u => u.Repository<OrderStatusHistory>().GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<OrderStatusHistory, bool>>>()))
                .ReturnsAsync(new List<OrderStatusHistory>());
            
            mapperMock.Setup(m => m.Map<IEnumerable<OrderDto>>(orders)).Returns(orderDtos);
            mapperMock.Setup(m => m.Map<ICollection<OrderItemDto>>(It.IsAny<IEnumerable<OrderItem>>())).Returns(new List<OrderItemDto>());
            mapperMock.Setup(m => m.Map<ICollection<OrderStatusHistoryDto>>(It.IsAny<IEnumerable<OrderStatusHistory>>())).Returns(new List<OrderStatusHistoryDto>());
            
            var service = new OrderService(unitOfWorkMock.Object, mapperMock.Object);
            
            // Act
            var result = await service.GetPendingOrdersAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, order => Assert.Equal("Pending", order.OrderStatus));
        }
    }
} 