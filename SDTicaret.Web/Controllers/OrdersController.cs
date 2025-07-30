using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text;
using System.Text.Json;

namespace SDTicaret.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("orders");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<OrderDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data ?? new List<OrderDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(new List<OrderDto>());
        }

        public async Task<IActionResult> Pending()
        {
            try
            {
                var response = await _httpClient.GetAsync("orders/pending");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<OrderDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View("Index", apiResponse?.Data ?? new List<OrderDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View("Index", new List<OrderDto>());
        }

        public async Task<IActionResult> Processing()
        {
            try
            {
                var response = await _httpClient.GetAsync("orders/processing");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<OrderDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View("Index", apiResponse?.Data ?? new List<OrderDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View("Index", new List<OrderDto>());
        }

        public async Task<IActionResult> Shipped()
        {
            try
            {
                var response = await _httpClient.GetAsync("orders/shipped");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<OrderDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View("Index", apiResponse?.Data ?? new List<OrderDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View("Index", new List<OrderDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<OrderDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return NotFound();
        }

        public async Task<IActionResult> History(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"orders/{id}/history");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<OrderStatusHistoryDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data ?? new List<OrderStatusHistoryDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(new List<OrderStatusHistoryDto>());
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                // Müşterileri getir
                var customersResponse = await _httpClient.GetAsync("customers");
                if (customersResponse.IsSuccessStatusCode)
                {
                    var customersContent = await customersResponse.Content.ReadAsStringAsync();
                    var customersApiResponse = JsonSerializer.Deserialize<ApiResponse<List<CustomerDto>>>(customersContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ViewBag.Customers = customersApiResponse?.Data ?? new List<CustomerDto>();
                }

                // Ürünleri getir
                var productsResponse = await _httpClient.GetAsync("products");
                if (productsResponse.IsSuccessStatusCode)
                {
                    var productsContent = await productsResponse.Content.ReadAsStringAsync();
                    var productsApiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductDto>>>(productsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ViewBag.Products = productsApiResponse?.Data ?? new List<ProductDto>();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("orders/create", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sipariş başarıyla oluşturuldu";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Hata: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus, string? notes = null, string? trackingNumber = null)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Auth");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var updateDto = new
                {
                    OrderId = id,
                    NewStatus = newStatus,
                    Notes = notes,
                    TrackingNumber = trackingNumber,
                    ChangedByUserId = HttpContext.Session.GetInt32("UserId"),
                    ChangedByUserName = HttpContext.Session.GetString("UserName")
                };

                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"orders/{id}/status", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sipariş durumu başarıyla güncellendi";
                }
                else
                {
                    TempData["ErrorMessage"] = "Sipariş durumu güncellenirken bir hata oluştu";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id, string reason)
        {
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Auth");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var cancelDto = new
                {
                    Reason = reason,
                    UserId = HttpContext.Session.GetInt32("UserId"),
                    UserName = HttpContext.Session.GetString("UserName")
                };

                var json = JsonSerializer.Serialize(cancelDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"orders/{id}/cancel", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sipariş başarıyla iptal edildi";
                }
                else
                {
                    TempData["ErrorMessage"] = "Sipariş iptal edilirken bir hata oluştu";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<OrderDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OrderDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"orders/{dto.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sipariş başarıyla güncellendi";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Hata: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sipariş başarıyla silindi";
                }
                else
                {
                    TempData["ErrorMessage"] = "Sipariş silinirken bir hata oluştu";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Hata: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 