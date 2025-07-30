using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text.Json;

namespace SDTicaret.Web.Controllers;

public class DashboardController : Controller
{
    private readonly HttpClient _httpClient;

    public DashboardController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            // Dashboard verilerini API'den al
            var response = await _httpClient.GetAsync("dashboard/stats");
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<DashboardStatsDto>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    var dashboardViewModel = new DashboardViewModel
                    {
                        TotalUsers = apiResponse.Data.TotalUsers,
                        TotalProducts = apiResponse.Data.TotalProducts,
                        TotalOrders = apiResponse.Data.TotalOrders,
                        TotalRevenue = apiResponse.Data.TotalRevenue,
                        PendingOrders = apiResponse.Data.PendingOrders,
                        ProcessingOrders = apiResponse.Data.ProcessingOrders,
                        ShippedOrders = apiResponse.Data.ShippedOrders,
                        DeliveredOrders = apiResponse.Data.DeliveredOrders,
                        LowStockProducts = apiResponse.Data.LowStockProducts,
                        OutOfStockProducts = apiResponse.Data.OutOfStockProducts,
                        MonthlyRevenue = apiResponse.Data.MonthlyRevenue,
                        WeeklyRevenue = apiResponse.Data.WeeklyRevenue,
                        DailyRevenue = apiResponse.Data.DailyRevenue,
                        RecentOrders = apiResponse.Data.RecentOrders?.Select(o => new OrderDto
                        {
                            Id = o.Id,
                            CustomerId = 0, // RecentOrderDto'da CustomerId yok
                            OrderDate = o.OrderDate,
                            TotalAmount = o.TotalAmount,
                            OrderStatus = o.Status // RecentOrderDto'da Status var, OrderStatus değil
                        }).ToList() ?? new List<OrderDto>(),
                        TopProducts = apiResponse.Data.TopProducts?.Select(p => new ProductDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            CategoryName = p.CategoryName
                        }).ToList() ?? new List<ProductDto>()
                    };

                    return View(dashboardViewModel);
                }
            }
        }
        catch (Exception)
        {
            // Hata durumunda varsayılan dashboard göster
        }

        // Varsayılan dashboard verileri
        var defaultDashboard = new DashboardViewModel
        {
            TotalUsers = 0,
            TotalProducts = 0,
            TotalOrders = 0,
            TotalRevenue = 0,
            RecentOrders = new List<OrderDto>(),
            TopProducts = new List<ProductDto>()
        };

        return View(defaultDashboard);
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> Analytics()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        var role = HttpContext.Session.GetString("Role");
        
        if (string.IsNullOrEmpty(token) || role != "Admin")
            return RedirectToAction("Login", "Auth");

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("dashboard/analytics");
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var analyticsData = JsonSerializer.Deserialize<AnalyticsViewModel>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(analyticsData);
            }
        }
        catch (Exception)
        {
            // Hata durumunda varsayılan analytics göster
        }

        return View(new AnalyticsViewModel());
    }
} 