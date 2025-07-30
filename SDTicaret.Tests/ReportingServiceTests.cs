using AutoMapper;
using Moq;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Services;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;
using System.Linq.Expressions;

namespace SDTicaret.Tests;

public class ReportingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ReportingService _reportingService;

    public ReportingServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _reportingService = new ReportingService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_Should_Return_Dashboard_Stats()
    {
        // Arrange
        var users = new List<User> { new User { Id = 1 }, new User { Id = 2 } };
        var products = new List<Product> { new Product { Id = 1 }, new Product { Id = 2 } };
        var orders = new List<Order> 
        { 
            new Order { Id = 1, TotalAmount = 100, OrderStatus = "Pending" },
            new Order { Id = 2, TotalAmount = 200, OrderStatus = "Delivered" }
        };
        var customers = new List<Customer> { new Customer { Id = 1 }, new Customer { Id = 2 } };
        var stocks = new List<Stock> 
        { 
            new Stock { Id = 1, Quantity = 0, MinimumStock = 5 },
            new Stock { Id = 2, Quantity = 3, MinimumStock = 5 }
        };

        _unitOfWorkMock.Setup(u => u.Repository<User>().GetAllAsync()).ReturnsAsync(users);
        _unitOfWorkMock.Setup(u => u.Repository<Product>().GetAllAsync()).ReturnsAsync(products);
        _unitOfWorkMock.Setup(u => u.Repository<Order>().GetAllAsync()).ReturnsAsync(orders);
        _unitOfWorkMock.Setup(u => u.Repository<Customer>().GetAllAsync()).ReturnsAsync(customers);
        _unitOfWorkMock.Setup(u => u.Repository<Stock>().GetAllAsync()).ReturnsAsync(stocks);

        _unitOfWorkMock.Setup(u => u.Repository<OrderItem>().GetAllAsync()).ReturnsAsync(new List<OrderItem>());
        _unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync()).ReturnsAsync(new List<Category>());
        _unitOfWorkMock.Setup(u => u.Repository<StockMovement>().GetAllAsync()).ReturnsAsync(new List<StockMovement>());

        _mapperMock.Setup(m => m.Map<List<RecentOrderDto>>(It.IsAny<IEnumerable<Order>>())).Returns(new List<RecentOrderDto>());
        _mapperMock.Setup(m => m.Map<List<StockMovementDto>>(It.IsAny<IEnumerable<StockMovement>>())).Returns(new List<StockMovementDto>());

        // Act
        var result = await _reportingService.GetDashboardStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalUsers);
        Assert.Equal(2, result.TotalProducts);
        Assert.Equal(2, result.TotalOrders);
        Assert.Equal(2, result.TotalCustomers);
        Assert.Equal(300, result.TotalRevenue);
        Assert.Equal(1, result.PendingOrders);
        Assert.Equal(1, result.DeliveredOrders);
        Assert.Equal(1, result.LowStockProducts);
        Assert.Equal(1, result.OutOfStockProducts);
    }

    [Fact]
    public async Task GetSalesReportAsync_Should_Return_Sales_Report()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        
        var orders = new List<Order> 
        { 
            new Order { Id = 1, TotalAmount = 100, OrderDate = DateTime.UtcNow, CustomerId = 1 },
            new Order { Id = 2, TotalAmount = 200, OrderDate = DateTime.UtcNow, CustomerId = 2 }
        };

        _unitOfWorkMock.Setup(u => u.Repository<Order>().GetAllAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(orders);
        _unitOfWorkMock.Setup(u => u.Repository<OrderItem>().GetAllAsync()).ReturnsAsync(new List<OrderItem>());
        _unitOfWorkMock.Setup(u => u.Repository<Product>().GetAllAsync()).ReturnsAsync(new List<Product>());
        _unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync()).ReturnsAsync(new List<Category>());

        // Act
        var result = await _reportingService.GetSalesReportAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(endDate, result.EndDate);
        Assert.Equal(300, result.TotalSales);
        Assert.Equal(2, result.TotalOrders);
        Assert.Equal(2, result.TotalCustomers);
        Assert.Equal(150, result.AverageOrderValue);
    }

    [Fact]
    public async Task GetInventoryReportAsync_Should_Return_Inventory_Report()
    {
        // Arrange
        var stocks = new List<Stock> 
        { 
            new Stock { Id = 1, Quantity = 0, MinimumStock = 5, MaximumStock = 100 },
            new Stock { Id = 2, Quantity = 3, MinimumStock = 5, MaximumStock = 100 },
            new Stock { Id = 3, Quantity = 150, MinimumStock = 5, MaximumStock = 100 }
        };
        var products = new List<Product> 
        { 
            new Product { Id = 1, Name = "Product 1", Price = 10, CategoryId = 1 },
            new Product { Id = 2, Name = "Product 2", Price = 20, CategoryId = 1 },
            new Product { Id = 3, Name = "Product 3", Price = 30, CategoryId = 1 }
        };
        var categories = new List<Category> { new Category { Id = 1, Name = "Category 1" } };

        _unitOfWorkMock.Setup(u => u.Repository<Stock>().GetAllAsync()).ReturnsAsync(stocks);
        _unitOfWorkMock.Setup(u => u.Repository<Product>().GetAllAsync()).ReturnsAsync(products);
        _unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync()).ReturnsAsync(categories);
        _unitOfWorkMock.Setup(u => u.Repository<StockMovement>().GetAllAsync()).ReturnsAsync(new List<StockMovement>());

        _mapperMock.Setup(m => m.Map<List<StockMovementDto>>(It.IsAny<IEnumerable<StockMovement>>())).Returns(new List<StockMovementDto>());

        // Act
        var result = await _reportingService.GetInventoryReportAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalProducts);
        Assert.Equal(1, result.LowStockProducts);
        Assert.Equal(1, result.OutOfStockProducts);
        Assert.Equal(1, result.OverstockProducts);
        Assert.Single(result.CategoryInventory);
    }

    [Fact]
    public async Task GetCustomerReportAsync_Should_Return_Customer_Report()
    {
        // Arrange
        var customers = new List<Customer> 
        { 
            new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", CreatedAt = DateTime.UtcNow.AddDays(-60) },
            new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", CreatedAt = DateTime.UtcNow.AddDays(-30) }
        };
        var orders = new List<Order> 
        { 
            new Order { Id = 1, CustomerId = 1, TotalAmount = 1000, OrderDate = DateTime.UtcNow.AddDays(-10) },
            new Order { Id = 2, CustomerId = 1, TotalAmount = 2000, OrderDate = DateTime.UtcNow.AddDays(-5) },
            new Order { Id = 3, CustomerId = 2, TotalAmount = 500, OrderDate = DateTime.UtcNow.AddDays(-15) }
        };

        _unitOfWorkMock.Setup(u => u.Repository<Customer>().GetAllAsync()).ReturnsAsync(customers);
        _unitOfWorkMock.Setup(u => u.Repository<Order>().GetAllAsync()).ReturnsAsync(orders);

        // Act
        var result = await _reportingService.GetCustomerReportAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCustomers);
        Assert.Equal(3500, result.TotalRevenue);
        Assert.Equal(1750, result.AverageCustomerValue);
        Assert.Equal(1, result.NewCustomersThisMonth);
        Assert.Equal(3, result.CustomerSegments.Count);
        Assert.Equal(2, result.CustomerActivity.Count);
        Assert.Equal(3, result.CustomerRetention.Count);
        Assert.Equal(2, result.CustomerLifetimeValue.Count);
    }

    [Fact]
    public async Task GetSalesReportAsync_With_Invalid_Dates_Should_Return_Empty_Report()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(-30); // End date before start date
        
        _unitOfWorkMock.Setup(u => u.Repository<Order>().GetAllAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(new List<Order>());

        // Act
        var result = await _reportingService.GetSalesReportAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalSales);
        Assert.Equal(0, result.TotalOrders);
        Assert.Equal(0, result.TotalCustomers);
        Assert.Equal(0, result.AverageOrderValue);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_With_No_Data_Should_Return_Zero_Stats()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Repository<User>().GetAllAsync()).ReturnsAsync(new List<User>());
        _unitOfWorkMock.Setup(u => u.Repository<Product>().GetAllAsync()).ReturnsAsync(new List<Product>());
        _unitOfWorkMock.Setup(u => u.Repository<Order>().GetAllAsync()).ReturnsAsync(new List<Order>());
        _unitOfWorkMock.Setup(u => u.Repository<Customer>().GetAllAsync()).ReturnsAsync(new List<Customer>());
        _unitOfWorkMock.Setup(u => u.Repository<Stock>().GetAllAsync()).ReturnsAsync(new List<Stock>());
        _unitOfWorkMock.Setup(u => u.Repository<OrderItem>().GetAllAsync()).ReturnsAsync(new List<OrderItem>());
        _unitOfWorkMock.Setup(u => u.Repository<Category>().GetAllAsync()).ReturnsAsync(new List<Category>());
        _unitOfWorkMock.Setup(u => u.Repository<StockMovement>().GetAllAsync()).ReturnsAsync(new List<StockMovement>());

        _mapperMock.Setup(m => m.Map<List<RecentOrderDto>>(It.IsAny<IEnumerable<Order>>())).Returns(new List<RecentOrderDto>());
        _mapperMock.Setup(m => m.Map<List<StockMovementDto>>(It.IsAny<IEnumerable<StockMovement>>())).Returns(new List<StockMovementDto>());

        // Act
        var result = await _reportingService.GetDashboardStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalUsers);
        Assert.Equal(0, result.TotalProducts);
        Assert.Equal(0, result.TotalOrders);
        Assert.Equal(0, result.TotalCustomers);
        Assert.Equal(0, result.TotalRevenue);
        Assert.Equal(0, result.PendingOrders);
        Assert.Equal(0, result.LowStockProducts);
        Assert.Equal(0, result.OutOfStockProducts);
    }
} 