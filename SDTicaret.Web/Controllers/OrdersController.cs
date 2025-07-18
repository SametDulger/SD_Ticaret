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

        public async Task<IActionResult> Create()
        {
            try
            {
                // Müşterileri yükle
                var customersResponse = await _httpClient.GetAsync("customers");
                var customers = new List<CustomerDto>();
                if (customersResponse.IsSuccessStatusCode)
                {
                    var customersContent = await customersResponse.Content.ReadAsStringAsync();
                    var customersApiResponse = JsonSerializer.Deserialize<ApiResponse<List<CustomerDto>>>(customersContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    customers = customersApiResponse?.Data ?? new List<CustomerDto>();
                }

                // Ürünleri yükle
                var productsResponse = await _httpClient.GetAsync("products");
                var products = new List<ProductDto>();
                if (productsResponse.IsSuccessStatusCode)
                {
                    var productsContent = await productsResponse.Content.ReadAsStringAsync();
                    var productsApiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductDto>>>(productsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    products = productsApiResponse?.Data ?? new List<ProductDto>();
                }

                ViewBag.Customers = customers;
                ViewBag.Products = products;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Veri yükleme hatası: {ex.Message}");
                ViewBag.Customers = new List<CustomerDto>();
                ViewBag.Products = new List<ProductDto>();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDto orderDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // OrderDate'i otomatik olarak ayarla
                    orderDto.OrderDate = DateTime.Now;
                    
                    var json = JsonSerializer.Serialize(orderDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("orders", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"API Hatası: {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Hata: {ex.Message}");
                }
            }
            
            // Hata durumunda dropdown'ları tekrar yükle
            try
            {
                var customersResponse = await _httpClient.GetAsync("customers");
                var customers = new List<CustomerDto>();
                if (customersResponse.IsSuccessStatusCode)
                {
                    var customersContent = await customersResponse.Content.ReadAsStringAsync();
                    var customersApiResponse = JsonSerializer.Deserialize<ApiResponse<List<CustomerDto>>>(customersContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    customers = customersApiResponse?.Data ?? new List<CustomerDto>();
                }

                var productsResponse = await _httpClient.GetAsync("products");
                var products = new List<ProductDto>();
                if (productsResponse.IsSuccessStatusCode)
                {
                    var productsContent = await productsResponse.Content.ReadAsStringAsync();
                    var productsApiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductDto>>>(productsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    products = productsApiResponse?.Data ?? new List<ProductDto>();
                }

                ViewBag.Customers = customers;
                ViewBag.Products = products;
            }
            catch (Exception ex)
            {
                ViewBag.Customers = new List<CustomerDto>();
                ViewBag.Products = new List<ProductDto>();
            }
            
            return View(orderDto);
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
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderDto orderDto)
        {
            if (id != orderDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(orderDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"orders/{id}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"API Hatası: {errorContent}");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Hata: {ex.Message}");
                }
            }
            return View(orderDto);
        }

        public async Task<IActionResult> Delete(int id)
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
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 