using AutoMapper;
using SDTicaret.Application.DTOs;
using SDTicaret.Application.Interfaces;
using SDTicaret.Core;
using SDTicaret.Core.Entities;
using SDTicaret.Core.Interfaces;
using System.Linq.Expressions;

namespace SDTicaret.Application.Services;

public class ReportingService : IReportingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReportingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var stats = new DashboardStatsDto();

        // Temel istatistikler
        var users = await _unitOfWork.Repository<User>().GetAllAsync();
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync();

        stats.TotalUsers = users.Count();
        stats.TotalProducts = products.Count();
        stats.TotalOrders = orders.Count();
        stats.TotalCustomers = customers.Count();
        stats.TotalRevenue = orders.Sum(o => o.TotalAmount);

        // Sipariş durumları
        stats.PendingOrders = orders.Count(o => o.OrderStatus == "Pending");
        stats.ProcessingOrders = orders.Count(o => o.OrderStatus == "Processing");
        stats.ShippedOrders = orders.Count(o => o.OrderStatus == "Shipped");
        stats.DeliveredOrders = orders.Count(o => o.OrderStatus == "Delivered");

        // Stok uyarıları
        stats.LowStockProducts = stocks.Count(s => s.Quantity <= s.MinimumStock && s.Quantity > 0);
        stats.OutOfStockProducts = stocks.Count(s => s.Quantity == 0);

        // Gelir istatistikleri
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
        var startOfDay = now.Date;

        stats.MonthlyRevenue = orders.Where(o => o.OrderDate >= startOfMonth).Sum(o => o.TotalAmount);
        stats.WeeklyRevenue = orders.Where(o => o.OrderDate >= startOfWeek).Sum(o => o.TotalAmount);
        stats.DailyRevenue = orders.Where(o => o.OrderDate >= startOfDay).Sum(o => o.TotalAmount);

        // Son siparişler
        var recentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10);
        stats.RecentOrders = _mapper.Map<List<RecentOrderDto>>(recentOrders);

        // En popüler ürünler
        var topProducts = await GetTopProductsAsync(10);
        stats.TopProducts = topProducts;

        // En iyi müşteriler
        var topCustomers = await GetTopCustomersAsync(10);
        stats.TopCustomers = topCustomers;

        // Stok uyarıları
        var stockAlerts = await GetStockAlertsAsync();
        stats.StockAlerts = stockAlerts;

        // Grafik verileri
        stats.RevenueChart = await GetRevenueChartDataAsync();
        stats.OrdersChart = await GetOrdersChartDataAsync();
        stats.CustomersChart = await GetCustomersChartDataAsync();

        return stats;
    }

    public async Task<SalesReportDto> GetSalesReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var now = DateTime.UtcNow;
        var actualStartDate = startDate ?? now.AddDays(-30);
        var actualEndDate = endDate ?? now;

        var report = new SalesReportDto
        {
            StartDate = actualStartDate,
            EndDate = actualEndDate
        };

        var orders = await _unitOfWork.Repository<Order>().GetAllAsync(o => 
            o.OrderDate >= actualStartDate && o.OrderDate <= actualEndDate && o.OrderStatus != "Cancelled");

        report.TotalSales = orders.Sum(o => o.TotalAmount);
        report.TotalOrders = orders.Count();
        report.TotalCustomers = orders.Select(o => o.CustomerId).Distinct().Count();
        report.AverageOrderValue = report.TotalOrders > 0 ? report.TotalSales / report.TotalOrders : 0;

        // Günlük satışlar
        report.DailySales = await GetDailySalesAsync(actualStartDate, actualEndDate);

        // Kategori satışları
        report.CategorySales = await GetCategorySalesAsync(actualStartDate, actualEndDate);

        // En iyi ürünler
        var topProducts = await GetTopProductsAsync(20);
        report.TopProducts = topProducts.Select(p => new ProductSalesDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            CategoryName = p.CategoryName,
            Sales = p.TotalSales,
            Quantity = p.SalesCount,
            Orders = 0, // Bu değer ayrıca hesaplanmalı
            UnitPrice = p.Price
        }).ToList();

        // En iyi müşteriler
        var topCustomers = await GetTopCustomersAsync(20);
        report.TopCustomers = topCustomers.Select(c => new CustomerSalesDto
        {
            CustomerId = c.Id,
            CustomerName = c.Name,
            Email = c.Email,
            TotalSpent = c.TotalSpent,
            Orders = c.OrderCount,
            LastOrderDate = c.LastOrderDate,
            AverageOrderValue = c.OrderCount > 0 ? c.TotalSpent / c.OrderCount : 0
        }).ToList();

        return report;
    }

    public async Task<InventoryReportDto> GetInventoryReportAsync()
    {
        var report = new InventoryReportDto();

        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync();
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

        report.TotalProducts = products.Count();
        report.TotalInventoryValue = stocks.Sum(s => s.Quantity * (products.FirstOrDefault(p => p.Id == s.ProductId)?.Price ?? 0));

        // Düşük stok ürünleri
        var lowStockItems = stocks.Where(s => s.Quantity <= s.MinimumStock && s.Quantity > 0).ToList();
        report.LowStockProducts = lowStockItems.Count;
        report.LowStockItems = await GetLowStockItemsAsync(lowStockItems);

        // Tükendi ürünleri
        var outOfStockItems = stocks.Where(s => s.Quantity == 0).ToList();
        report.OutOfStockProducts = outOfStockItems.Count;
        report.OutOfStockItems = await GetOutOfStockItemsAsync(outOfStockItems);

        // Aşırı stok ürünleri
        var overstockItems = stocks.Where(s => s.Quantity > s.MaximumStock).ToList();
        report.OverstockProducts = overstockItems.Count;
        report.OverstockItems = await GetOverstockItemsAsync(overstockItems);

        // Kategori envanteri
        report.CategoryInventory = await GetCategoryInventoryAsync();

        // Son stok hareketleri
        var recentMovements = await _unitOfWork.Repository<StockMovement>().GetAllAsync();
        report.RecentMovements = _mapper.Map<List<StockMovementDto>>(recentMovements.OrderByDescending(m => m.CreatedAt).Take(20));

        return report;
    }

    public async Task<CustomerReportDto> GetCustomerReportAsync()
    {
        var report = new CustomerReportDto();

        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();

        report.TotalCustomers = customers.Count();
        report.TotalRevenue = orders.Sum(o => o.TotalAmount);
        report.AverageCustomerValue = report.TotalCustomers > 0 ? report.TotalRevenue / report.TotalCustomers : 0;

        // Aktif müşteriler (son 30 günde sipariş veren)
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        report.ActiveCustomers = orders.Where(o => o.OrderDate >= thirtyDaysAgo)
            .Select(o => o.CustomerId).Distinct().Count();

        // Bu ay yeni müşteriler
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        report.NewCustomersThisMonth = customers.Count(c => c.CreatedAt >= startOfMonth);

        // Müşteri segmentleri
        report.CustomerSegments = await GetCustomerSegmentsAsync();

        // Müşteri aktivitesi
        report.CustomerActivity = await GetCustomerActivityAsync();

        // Müşteri tutma oranları
        report.CustomerRetention = await GetCustomerRetentionAsync();

        // Müşteri yaşam boyu değeri
        report.CustomerLifetimeValue = await GetCustomerLifetimeValueAsync();

        return report;
    }

    private async Task<List<TopProductDto>> GetTopProductsAsync(int count)
    {
        var orderItems = await _unitOfWork.Repository<OrderItem>().GetAllAsync();
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

        var topProducts = orderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                SalesCount = g.Sum(oi => oi.Quantity),
                TotalSales = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(x => x.TotalSales)
            .Take(count)
            .ToList();

        var result = new List<TopProductDto>();
        foreach (var item in topProducts)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
                result.Add(new TopProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    CategoryName = category?.Name ?? "",
                    Price = product.Price,
                    SalesCount = item.SalesCount,
                    TotalSales = item.TotalSales
                });
            }
        }

        return result;
    }

    private async Task<List<TopCustomerDto>> GetTopCustomersAsync(int count)
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();

        var topCustomers = orders
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                OrderCount = g.Count(),
                TotalSpent = g.Sum(o => o.TotalAmount),
                LastOrderDate = g.Max(o => o.OrderDate)
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(count)
            .ToList();

        var result = new List<TopCustomerDto>();
        foreach (var item in topCustomers)
        {
            var customer = customers.FirstOrDefault(c => c.Id == item.CustomerId);
            if (customer != null)
            {
                result.Add(new TopCustomerDto
                {
                    Id = customer.Id,
                    Name = $"{customer.FirstName ?? ""} {customer.LastName ?? ""}".Trim(),
                    Email = customer.Email ?? "",
                    OrderCount = item.OrderCount,
                    TotalSpent = item.TotalSpent,
                    LastOrderDate = item.LastOrderDate
                });
            }
        }

        return result;
    }

    private async Task<List<StockAlertDto>> GetStockAlertsAsync()
    {
        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync();
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

        var alerts = new List<StockAlertDto>();

        foreach (var stock in stocks)
        {
            var product = products.FirstOrDefault(p => p.Id == stock.ProductId);
            if (product == null) continue;

            var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);

            if (stock.Quantity == 0)
            {
                alerts.Add(new StockAlertDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    CategoryName = category?.Name ?? "",
                    CurrentStock = stock.Quantity,
                    MinimumStock = stock.MinimumStock,
                    AlertType = "Out",
                    Message = "Ürün stokta tükendi"
                });
            }
            else if (stock.Quantity <= stock.MinimumStock)
            {
                alerts.Add(new StockAlertDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    CategoryName = category?.Name ?? "",
                    CurrentStock = stock.Quantity,
                    MinimumStock = stock.MinimumStock,
                    AlertType = "Low",
                    Message = "Ürün stoku kritik seviyede"
                });
            }
        }

        return alerts.OrderBy(a => a.AlertType).Take(10).ToList();
    }

    private async Task<List<ChartDataDto>> GetRevenueChartDataAsync()
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
        var last30Days = Enumerable.Range(0, 30)
            .Select(i => DateTime.UtcNow.AddDays(-i).Date)
            .Reverse()
            .ToList();

        var chartData = new List<ChartDataDto>();
        foreach (var date in last30Days)
        {
            var dayOrders = orders.Where(o => o.OrderDate.Date == date);
            chartData.Add(new ChartDataDto
            {
                Label = date.ToString("dd/MM"),
                Value = dayOrders.Sum(o => o.TotalAmount),
                Count = dayOrders.Count(),
                Date = date
            });
        }

        return chartData;
    }

    private async Task<List<ChartDataDto>> GetOrdersChartDataAsync()
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.UtcNow.AddDays(-i).Date)
            .Reverse()
            .ToList();

        var chartData = new List<ChartDataDto>();
        foreach (var date in last7Days)
        {
            var dayOrders = orders.Where(o => o.OrderDate.Date == date);
            chartData.Add(new ChartDataDto
            {
                Label = date.ToString("dd/MM"),
                Value = dayOrders.Sum(o => o.TotalAmount),
                Count = dayOrders.Count(),
                Date = date
            });
        }

        return chartData;
    }

    private async Task<List<ChartDataDto>> GetCustomersChartDataAsync()
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        var last12Months = Enumerable.Range(0, 12)
            .Select(i => DateTime.UtcNow.AddMonths(-i))
            .Reverse()
            .ToList();

        var chartData = new List<ChartDataDto>();
        foreach (var month in last12Months)
        {
            var monthCustomers = customers.Where(c => c.CreatedAt.Month == month.Month && c.CreatedAt.Year == month.Year);
            chartData.Add(new ChartDataDto
            {
                Label = month.ToString("MMM yyyy"),
                Value = 0,
                Count = monthCustomers.Count(),
                Date = month
            });
        }

        return chartData;
    }

    private async Task<List<DailySalesDto>> GetDailySalesAsync(DateTime startDate, DateTime endDate)
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync(o => 
            o.OrderDate >= startDate && o.OrderDate <= endDate);

        var dailySales = orders
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new DailySalesDto
            {
                Date = g.Key,
                Sales = g.Sum(o => o.TotalAmount),
                Orders = g.Count(),
                Customers = g.Select(o => o.CustomerId).Distinct().Count()
            })
            .OrderBy(x => x.Date)
            .ToList();

        return dailySales;
    }

    private async Task<List<CategorySalesDto>> GetCategorySalesAsync(DateTime startDate, DateTime endDate)
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync(o => 
            o.OrderDate >= startDate && o.OrderDate <= endDate);
        var orderItems = await _unitOfWork.Repository<OrderItem>().GetAllAsync();
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

        var orderIds = orders.Select(o => o.Id).ToList();
        var relevantOrderItems = orderItems.Where(oi => orderIds.Contains(oi.OrderId)).ToList();

        var categorySales = relevantOrderItems
            .Join(products, oi => oi.ProductId, p => p.Id, (oi, p) => new { oi, p })
            .Join(categories, x => x.p.CategoryId, c => c.Id, (x, c) => new { x.oi, x.p, c })
            .GroupBy(x => x.c.Id)
            .Select(g => new CategorySalesDto
            {
                CategoryId = g.Key,
                CategoryName = g.First().c.Name,
                Sales = g.Sum(x => x.oi.Quantity * x.oi.UnitPrice),
                Orders = g.Select(x => x.oi.OrderId).Distinct().Count(),
                Products = g.Select(x => x.p.Id).Distinct().Count(),
                Percentage = 0 // Hesaplanacak
            })
            .OrderByDescending(x => x.Sales)
            .ToList();

        var totalSales = categorySales.Sum(x => x.Sales);
        foreach (var category in categorySales)
        {
            category.Percentage = totalSales > 0 ? (category.Sales / totalSales) * 100 : 0;
        }

        return categorySales;
    }

    private async Task<List<LowStockProductDto>> GetLowStockItemsAsync(List<Stock> lowStockItems)
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

        var result = new List<LowStockProductDto>();
        foreach (var stock in lowStockItems)
        {
            var product = products.FirstOrDefault(p => p.Id == stock.ProductId);
            if (product == null) continue;

            var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            result.Add(new LowStockProductDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                CategoryName = category?.Name ?? "",
                CurrentStock = stock.Quantity,
                MinimumStock = stock.MinimumStock,
                MaximumStock = stock.MaximumStock,
                UnitPrice = product.Price,
                StockValue = stock.Quantity * product.Price,
                LastStockIn = stock.LastStockInDate ?? DateTime.MinValue,
                LastStockOut = stock.LastStockOutDate ?? DateTime.MinValue
            });
        }

        return result.OrderBy(x => x.CurrentStock).ToList();
    }

    private async Task<List<OutOfStockProductDto>> GetOutOfStockItemsAsync(List<Stock> outOfStockItems)
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

        var result = new List<OutOfStockProductDto>();
        foreach (var stock in outOfStockItems)
        {
            var product = products.FirstOrDefault(p => p.Id == stock.ProductId);
            if (product == null) continue;

            var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            var daysOutOfStock = stock.LastStockOutDate.HasValue ? 
                (DateTime.UtcNow - stock.LastStockOutDate.Value).Days : 0;

            result.Add(new OutOfStockProductDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                CategoryName = category?.Name ?? "",
                CurrentStock = stock.Quantity,
                MinimumStock = stock.MinimumStock,
                UnitPrice = product.Price,
                LastStockOut = stock.LastStockOutDate ?? DateTime.MinValue,
                DaysOutOfStock = daysOutOfStock
            });
        }

        return result.OrderByDescending(x => x.DaysOutOfStock).ToList();
    }

    private async Task<List<OverstockProductDto>> GetOverstockItemsAsync(List<Stock> overstockItems)
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

        var result = new List<OverstockProductDto>();
        foreach (var stock in overstockItems)
        {
            var product = products.FirstOrDefault(p => p.Id == stock.ProductId);
            if (product == null) continue;

            var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            var excessStock = stock.Quantity - stock.MaximumStock;

            result.Add(new OverstockProductDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                CategoryName = category?.Name ?? "",
                CurrentStock = stock.Quantity,
                MaximumStock = stock.MaximumStock,
                UnitPrice = product.Price,
                StockValue = stock.Quantity * product.Price,
                ExcessStock = excessStock,
                ExcessValue = excessStock * product.Price
            });
        }

        return result.OrderByDescending(x => x.ExcessValue).ToList();
    }

    private async Task<List<CategoryInventoryDto>> GetCategoryInventoryAsync()
    {
        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync();
        var products = await _unitOfWork.Repository<Product>().GetAllAsync();
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

        var categoryInventory = categories.Select(category =>
        {
            var categoryProducts = products.Where(p => p.CategoryId == category.Id).ToList();
            var categoryStocks = stocks.Where(s => categoryProducts.Any(p => p.Id == s.ProductId)).ToList();

            return new CategoryInventoryDto
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                TotalProducts = categoryProducts.Count,
                LowStockProducts = categoryStocks.Count(s => s.Quantity <= s.MinimumStock && s.Quantity > 0),
                OutOfStockProducts = categoryStocks.Count(s => s.Quantity == 0),
                OverstockProducts = categoryStocks.Count(s => s.Quantity > s.MaximumStock),
                TotalValue = categoryStocks.Sum(s => s.Quantity * (categoryProducts.FirstOrDefault(p => p.Id == s.ProductId)?.Price ?? 0)),
                AverageValue = categoryStocks.Any() ? categoryStocks.Sum(s => s.Quantity * (categoryProducts.FirstOrDefault(p => p.Id == s.ProductId)?.Price ?? 0)) / categoryStocks.Count : 0
            };
        }).ToList();

        return categoryInventory.OrderByDescending(x => x.TotalValue).ToList();
    }

    private async Task<List<CustomerSegmentDto>> GetCustomerSegmentsAsync()
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();

        var customerStats = orders
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                TotalRevenue = g.Sum(o => o.TotalAmount),
                TotalOrders = g.Count(),
                AverageOrderValue = g.Average(o => o.TotalAmount)
            })
            .ToList();

        var totalRevenue = customerStats.Sum(x => x.TotalRevenue);
        var avgRevenue = customerStats.Any() ? customerStats.Average(x => x.TotalRevenue) : 0m;

        var segments = new List<CustomerSegmentDto>();

        // High value customers (>150% of average)
        var highValueCustomers = customerStats.Where(x => x.TotalRevenue > avgRevenue * 1.5m).ToList();
        segments.Add(new CustomerSegmentDto
        {
            Segment = "High",
            CustomerCount = highValueCustomers.Count,
            TotalRevenue = highValueCustomers.Sum(x => x.TotalRevenue),
            AverageOrderValue = highValueCustomers.Any() ? highValueCustomers.Average(x => x.AverageOrderValue) : 0,
            TotalOrders = highValueCustomers.Sum(x => x.TotalOrders),
            Percentage = totalRevenue > 0 ? (highValueCustomers.Sum(x => x.TotalRevenue) / totalRevenue) * 100 : 0
        });

        // Medium value customers (50-150% of average)
        var mediumValueCustomers = customerStats.Where(x => x.TotalRevenue >= avgRevenue * 0.5m && x.TotalRevenue <= avgRevenue * 1.5m).ToList();
        segments.Add(new CustomerSegmentDto
        {
            Segment = "Medium",
            CustomerCount = mediumValueCustomers.Count,
            TotalRevenue = mediumValueCustomers.Sum(x => x.TotalRevenue),
            AverageOrderValue = mediumValueCustomers.Any() ? mediumValueCustomers.Average(x => x.AverageOrderValue) : 0,
            TotalOrders = mediumValueCustomers.Sum(x => x.TotalOrders),
            Percentage = totalRevenue > 0 ? (mediumValueCustomers.Sum(x => x.TotalRevenue) / totalRevenue) * 100 : 0
        });

        // Low value customers (<50% of average)
        var lowValueCustomers = customerStats.Where(x => x.TotalRevenue < avgRevenue * 0.5m).ToList();
        segments.Add(new CustomerSegmentDto
        {
            Segment = "Low",
            CustomerCount = lowValueCustomers.Count,
            TotalRevenue = lowValueCustomers.Sum(x => x.TotalRevenue),
            AverageOrderValue = lowValueCustomers.Any() ? lowValueCustomers.Average(x => x.AverageOrderValue) : 0,
            TotalOrders = lowValueCustomers.Sum(x => x.TotalOrders),
            Percentage = totalRevenue > 0 ? (lowValueCustomers.Sum(x => x.TotalRevenue) / totalRevenue) * 100 : 0
        });

        return segments;
    }

    private async Task<List<CustomerActivityDto>> GetCustomerActivityAsync()
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();

        var customerActivity = new List<CustomerActivityDto>();

        foreach (var customer in customers)
        {
            var customerOrders = orders.Where(o => o.CustomerId == customer.Id).ToList();
            var lastOrder = customerOrders.OrderByDescending(o => o.OrderDate).FirstOrDefault();
            var daysSinceLastOrder = lastOrder != null ? (DateTime.UtcNow - lastOrder.OrderDate).Days : 0;

            string activityStatus;
            if (customerOrders.Count == 0)
                activityStatus = "New";
            else if (daysSinceLastOrder <= 30)
                activityStatus = "Active";
            else
                activityStatus = "Inactive";

            customerActivity.Add(new CustomerActivityDto
            {
                CustomerId = customer.Id,
                CustomerName = $"{customer.FirstName ?? ""} {customer.LastName ?? ""}".Trim(),
                Email = customer.Email ?? "",
                RegistrationDate = customer.CreatedAt,
                LastOrderDate = lastOrder?.OrderDate ?? customer.CreatedAt,
                TotalOrders = customerOrders.Count,
                TotalSpent = customerOrders.Sum(o => o.TotalAmount),
                ActivityStatus = activityStatus,
                DaysSinceLastOrder = daysSinceLastOrder
            });
        }

        return customerActivity.OrderByDescending(x => x.TotalSpent).Take(50).ToList();
    }

    private async Task<List<CustomerRetentionDto>> GetCustomerRetentionAsync()
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();

        var periods = new[] { 30, 60, 90 };
        var retentionData = new List<CustomerRetentionDto>();

        foreach (var period in periods)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-period);
            var oldCustomers = customers.Where(c => c.CreatedAt < cutoffDate).Count();
            var retainedCustomers = orders
                .Where(o => o.OrderDate >= cutoffDate)
                .Select(o => o.CustomerId)
                .Distinct()
                .Count(customerId => customers.Any(c => c.Id == customerId && c.CreatedAt < cutoffDate));

            var newCustomers = customers.Count(c => c.CreatedAt >= cutoffDate);
            var lostCustomers = oldCustomers - retainedCustomers;

            retentionData.Add(new CustomerRetentionDto
            {
                Period = $"{period} days",
                TotalCustomers = oldCustomers + newCustomers,
                RetainedCustomers = retainedCustomers,
                NewCustomers = newCustomers,
                LostCustomers = lostCustomers,
                RetentionRate = oldCustomers > 0 ? (decimal)retainedCustomers / oldCustomers * 100 : 0,
                ChurnRate = oldCustomers > 0 ? (decimal)lostCustomers / oldCustomers * 100 : 0
            });
        }

        return retentionData;
    }

    private async Task<List<CustomerLifetimeValueDto>> GetCustomerLifetimeValueAsync()
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync();
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();

        var lifetimeValues = new List<CustomerLifetimeValueDto>();

        foreach (var customer in customers)
        {
            var customerOrders = orders.Where(o => o.CustomerId == customer.Id).ToList();
            if (!customerOrders.Any()) continue;

            var firstOrder = customerOrders.OrderBy(o => o.OrderDate).First();
            var lastOrder = customerOrders.OrderByDescending(o => o.OrderDate).First();
            var totalRevenue = customerOrders.Sum(o => o.TotalAmount);
            var averageOrderValue = customerOrders.Average(o => o.TotalAmount);
            var lifetimeDays = (lastOrder.OrderDate - firstOrder.OrderDate).Days;

            string valueTier;
            if (totalRevenue >= 10000)
                valueTier = "Platinum";
            else if (totalRevenue >= 5000)
                valueTier = "Gold";
            else if (totalRevenue >= 1000)
                valueTier = "Silver";
            else
                valueTier = "Bronze";

            lifetimeValues.Add(new CustomerLifetimeValueDto
            {
                CustomerId = customer.Id,
                CustomerName = $"{customer.FirstName ?? ""} {customer.LastName ?? ""}".Trim(),
                Email = customer.Email ?? "",
                FirstOrderDate = firstOrder.OrderDate,
                LastOrderDate = lastOrder.OrderDate,
                TotalOrders = customerOrders.Count,
                TotalRevenue = totalRevenue,
                AverageOrderValue = averageOrderValue,
                CustomerLifetimeDays = lifetimeDays,
                LifetimeValue = totalRevenue,
                ValueTier = valueTier
            });
        }

        return lifetimeValues.OrderByDescending(x => x.LifetimeValue).Take(50).ToList();
    }

    public async Task<List<OrderDto>> GetRecentOrdersAsync(int count = 5)
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
        var recentOrders = orders
            .OrderByDescending(o => o.OrderDate)
            .Take(count)
            .ToList();

        return _mapper.Map<List<OrderDto>>(recentOrders);
    }

    public async Task<List<ProductDto>> GetLowStockProductsAsync(int count = 10)
    {
        var stocks = await _unitOfWork.Repository<Stock>().GetAllAsync();
        var lowStockStocks = stocks
            .Where(s => s.Quantity <= s.MinimumStock && s.Quantity > 0)
            .OrderBy(s => s.Quantity)
            .Take(count)
            .ToList();

        var productIds = lowStockStocks.Select(s => s.ProductId).ToList();
        var products = await _unitOfWork.Repository<Product>().GetAllAsync(p => productIds.Contains(p.Id));

        return _mapper.Map<List<ProductDto>>(products);
    }
} 