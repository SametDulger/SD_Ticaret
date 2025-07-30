namespace SDTicaret.Web.Models;

public class InventoryReportDto
{
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public int OverstockProducts { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public List<LowStockProductDto> LowStockItems { get; set; } = new();
    public List<OutOfStockProductDto> OutOfStockItems { get; set; } = new();
    public List<OverstockProductDto> OverstockItems { get; set; } = new();
    public List<CategoryInventoryDto> CategoryInventory { get; set; } = new();
    public List<StockMovementDto> RecentMovements { get; set; } = new();
}

public class LowStockProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal StockValue { get; set; }
    public DateTime LastStockIn { get; set; }
    public DateTime LastStockOut { get; set; }
}

public class OutOfStockProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime LastStockOut { get; set; }
    public int DaysOutOfStock { get; set; }
}

public class OverstockProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MaximumStock { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal StockValue { get; set; }
    public int ExcessStock { get; set; }
    public decimal ExcessValue { get; set; }
}

public class CategoryInventoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public int OverstockProducts { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageValue { get; set; }
} 