using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text;
using System.Text.Json;

namespace SDTicaret.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("products");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data ?? new List<ProductDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(new List<ProductDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                // Kategorileri yükle
                var categoriesResponse = await _httpClient.GetAsync("categories");
                var categories = new List<CategoryDto>();
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    var categoriesContent = await categoriesResponse.Content.ReadAsStringAsync();
                    var categoriesApiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(categoriesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    categories = categoriesApiResponse?.Data ?? new List<CategoryDto>();
                }

                // Tedarikçileri yükle
                var suppliersResponse = await _httpClient.GetAsync("suppliers");
                var suppliers = new List<SupplierDto>();
                if (suppliersResponse.IsSuccessStatusCode)
                {
                    var suppliersContent = await suppliersResponse.Content.ReadAsStringAsync();
                    var suppliersApiResponse = JsonSerializer.Deserialize<ApiResponse<List<SupplierDto>>>(suppliersContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    suppliers = suppliersApiResponse?.Data ?? new List<SupplierDto>();
                }

                ViewBag.Categories = categories;
                ViewBag.Suppliers = suppliers;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Veri yükleme hatası: {ex.Message}");
                ViewBag.Categories = new List<CategoryDto>();
                ViewBag.Suppliers = new List<SupplierDto>();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            // Debug için log
            Console.WriteLine($"Create action called. ModelState.IsValid: {ModelState.IsValid}");
            Console.WriteLine($"ProductDto: {JsonSerializer.Serialize(productDto)}");
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"ModelState errors: {string.Join(", ", errors)}");
                return View(productDto);
            }
            
            try
            {
                Console.WriteLine($"Sending request to API. Product: {JsonSerializer.Serialize(productDto)}");
                var json = JsonSerializer.Serialize(productDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                // HTTP Client'ın BaseAddress'ini kontrol et
                Console.WriteLine($"HTTP Client BaseAddress: {_httpClient.BaseAddress}");
                Console.WriteLine($"Full URL: {_httpClient.BaseAddress}products");
                
                var response = await _httpClient.PostAsync("products", content);
                
                Console.WriteLine($"API Response Status: {response.StatusCode}");
                Console.WriteLine($"API Response Headers: {JsonSerializer.Serialize(response.Headers)}");
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("API request successful");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {errorContent}");
                    ModelState.AddModelError("", $"API Hatası: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            
            // Hata durumunda kategoriler ve tedarikçileri tekrar yükle
            try
            {
                var categoriesResponse = await _httpClient.GetAsync("categories");
                var categories = new List<CategoryDto>();
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    var categoriesContent = await categoriesResponse.Content.ReadAsStringAsync();
                    var categoriesApiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(categoriesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    categories = categoriesApiResponse?.Data ?? new List<CategoryDto>();
                }

                var suppliersResponse = await _httpClient.GetAsync("suppliers");
                var suppliers = new List<SupplierDto>();
                if (suppliersResponse.IsSuccessStatusCode)
                {
                    var suppliersContent = await suppliersResponse.Content.ReadAsStringAsync();
                    var suppliersApiResponse = JsonSerializer.Deserialize<ApiResponse<List<SupplierDto>>>(suppliersContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    suppliers = suppliersApiResponse?.Data ?? new List<SupplierDto>();
                }

                ViewBag.Categories = categories;
                ViewBag.Suppliers = suppliers;
            }
            catch (Exception)
            {
                ViewBag.Categories = new List<CategoryDto>();
                ViewBag.Suppliers = new List<SupplierDto>();
            }
            
            return View(productDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    // Kategorileri yükle
                    var categoriesResponse = await _httpClient.GetAsync("categories");
                    var categories = new List<CategoryDto>();
                    if (categoriesResponse.IsSuccessStatusCode)
                    {
                        var categoriesContent = await categoriesResponse.Content.ReadAsStringAsync();
                        var categoriesApiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(categoriesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        categories = categoriesApiResponse?.Data ?? new List<CategoryDto>();
                    }

                    // Tedarikçileri yükle
                    var suppliersResponse = await _httpClient.GetAsync("suppliers");
                    var suppliers = new List<SupplierDto>();
                    if (suppliersResponse.IsSuccessStatusCode)
                    {
                        var suppliersContent = await suppliersResponse.Content.ReadAsStringAsync();
                        var suppliersApiResponse = JsonSerializer.Deserialize<ApiResponse<List<SupplierDto>>>(suppliersContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        suppliers = suppliersApiResponse?.Data ?? new List<SupplierDto>();
                    }

                    ViewBag.Categories = categories;
                    ViewBag.Suppliers = suppliers;
                    
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
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(productDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"products/{id}", content);
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
            
            // Hata durumunda kategoriler ve tedarikçileri tekrar yükle
            try
            {
                var categoriesResponse = await _httpClient.GetAsync("categories");
                var categories = new List<CategoryDto>();
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    var categoriesContent = await categoriesResponse.Content.ReadAsStringAsync();
                    var categoriesApiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(categoriesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    categories = categoriesApiResponse?.Data ?? new List<CategoryDto>();
                }

                var suppliersResponse = await _httpClient.GetAsync("suppliers");
                var suppliers = new List<SupplierDto>();
                if (suppliersResponse.IsSuccessStatusCode)
                {
                    var suppliersContent = await suppliersResponse.Content.ReadAsStringAsync();
                    var suppliersApiResponse = JsonSerializer.Deserialize<ApiResponse<List<SupplierDto>>>(suppliersContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    suppliers = suppliersApiResponse?.Data ?? new List<SupplierDto>();
                }

                ViewBag.Categories = categories;
                ViewBag.Suppliers = suppliers;
            }
            catch (Exception)
            {
                ViewBag.Categories = new List<CategoryDto>();
                ViewBag.Suppliers = new List<SupplierDto>();
            }
            
            return View(productDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                var response = await _httpClient.DeleteAsync($"products/{id}");
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