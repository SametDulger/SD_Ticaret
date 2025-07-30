namespace SDTicaret.Application.DTOs;

public class SalesReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<DailySalesDto> DailySales { get; set; } = new();
    public List<CategorySalesDto> CategorySales { get; set; } = new();
    public List<ProductSalesDto> TopProducts { get; set; } = new();
    public List<CustomerSalesDto> TopCustomers { get; set; } = new();
}

public class DailySalesDto
{
    public DateTime Date { get; set; }
    public decimal Sales { get; set; }
    public int Orders { get; set; }
    public int Customers { get; set; }
}

public class CategorySalesDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public int Orders { get; set; }
    public int Products { get; set; }
    public decimal Percentage { get; set; }
}

public class ProductSalesDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public int Quantity { get; set; }
    public int Orders { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CustomerSalesDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int Orders { get; set; }
    public DateTime LastOrderDate { get; set; }
    public decimal AverageOrderValue { get; set; }
} 