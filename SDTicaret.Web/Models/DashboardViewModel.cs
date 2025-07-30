namespace SDTicaret.Web.Models;

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal WeeklyRevenue { get; set; }
    public decimal DailyRevenue { get; set; }
    public List<OrderDto> RecentOrders { get; set; } = new();
    public List<ProductDto> TopProducts { get; set; } = new();
}

public class AnalyticsViewModel
{
    public List<ChartData> MonthlySales { get; set; } = new();
    public List<ChartData> ProductCategories { get; set; } = new();
    public List<ChartData> UserRegistrations { get; set; } = new();
}

public class ChartData
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
} 