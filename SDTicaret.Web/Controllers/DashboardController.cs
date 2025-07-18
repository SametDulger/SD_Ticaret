using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text.Json;

namespace SDTicaret.Web.Controllers;

public class DashboardController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public DashboardController(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5080/api/");
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
            var response = await _httpClient.GetAsync("api/dashboard/stats");
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var dashboardData = JsonSerializer.Deserialize<DashboardViewModel>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(dashboardData);
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
            var response = await _httpClient.GetAsync("api/dashboard/analytics");
            
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