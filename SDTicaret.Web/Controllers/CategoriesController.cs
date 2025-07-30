using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text;
using System.Text.Json;

namespace SDTicaret.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("categories");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data ?? new List<CategoryDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(new List<CategoryDto>());
        }

        public async Task<IActionResult> Tree()
        {
            try
            {
                var response = await _httpClient.GetAsync("categories/tree");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data ?? new List<CategoryDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(new List<CategoryDto>());
        }

        public async Task<IActionResult> SubCategories(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"categories/{id}/subcategories");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data ?? new List<CategoryDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(new List<CategoryDto>());
        }

        public async Task<IActionResult> ProductsByCategory(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"categories/{id}/products");
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
                var response = await _httpClient.GetAsync($"categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                // Ana kategorileri getir
                var response = await _httpClient.GetAsync("categories/main");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ViewBag.MainCategories = apiResponse?.Data ?? new List<CategoryDto>();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(categoryDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("categories", content);
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
            return View(categoryDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    // Ana kategorileri getir
                    var mainCategoriesResponse = await _httpClient.GetAsync("categories/main");
                    if (mainCategoriesResponse.IsSuccessStatusCode)
                    {
                        var mainContent = await mainCategoriesResponse.Content.ReadAsStringAsync();
                        var mainApiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryDto>>>(mainContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        ViewBag.MainCategories = mainApiResponse?.Data ?? new List<CategoryDto>();
                    }
                    
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
        public async Task<IActionResult> Edit(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(categoryDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"categories/{id}", content);
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
            return View(categoryDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                var response = await _httpClient.DeleteAsync($"categories/{id}");
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