namespace SDTicaret.Application.DTOs;

public class CustomerReportDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageCustomerValue { get; set; }
    public List<CustomerSegmentDto> CustomerSegments { get; set; } = new();
    public List<CustomerActivityDto> CustomerActivity { get; set; } = new();
    public List<CustomerRetentionDto> CustomerRetention { get; set; } = new();
    public List<CustomerLifetimeValueDto> CustomerLifetimeValue { get; set; } = new();
}

public class CustomerSegmentDto
{
    public string Segment { get; set; } = string.Empty; // "High", "Medium", "Low"
    public int CustomerCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalOrders { get; set; }
    public decimal Percentage { get; set; }
}

public class CustomerActivityDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public DateTime LastOrderDate { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public string ActivityStatus { get; set; } = string.Empty; // "Active", "Inactive", "New"
    public int DaysSinceLastOrder { get; set; }
}

public class CustomerRetentionDto
{
    public string Period { get; set; } = string.Empty; // "30 days", "60 days", "90 days"
    public int TotalCustomers { get; set; }
    public int RetainedCustomers { get; set; }
    public int NewCustomers { get; set; }
    public int LostCustomers { get; set; }
    public decimal RetentionRate { get; set; }
    public decimal ChurnRate { get; set; }
}

public class CustomerLifetimeValueDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FirstOrderDate { get; set; }
    public DateTime LastOrderDate { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int CustomerLifetimeDays { get; set; }
    public decimal LifetimeValue { get; set; }
    public string ValueTier { get; set; } = string.Empty; // "Platinum", "Gold", "Silver", "Bronze"
} 