using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text;
using System.Text.Json;

namespace SDTicaret.Web.Controllers
{
    public class StocksController : Controller
    {
        private readonly HttpClient _httpClient;

        public StocksController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("stocks");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<StockDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data ?? new List<StockDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(new List<StockDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"stocks/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<StockDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                // Ürünleri yükle
                var productsResponse = await _httpClient.GetAsync("products");
                var products = new List<ProductDto>();
                if (productsResponse.IsSuccessStatusCode)
                {
                    var productsContent = await productsResponse.Content.ReadAsStringAsync();
                    var productsApiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductDto>>>(productsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    products = productsApiResponse?.Data ?? new List<ProductDto>();
                }

                // Şubeleri yükle
                var branchesResponse = await _httpClient.GetAsync("branches");
                var branches = new List<BranchDto>();
                if (branchesResponse.IsSuccessStatusCode)
                {
                    var branchesContent = await branchesResponse.Content.ReadAsStringAsync();
                    var branchesApiResponse = JsonSerializer.Deserialize<ApiResponse<List<BranchDto>>>(branchesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    branches = branchesApiResponse?.Data ?? new List<BranchDto>();
                }

                ViewBag.Products = products;
                ViewBag.Branches = branches;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Veri yükleme hatası: {ex.Message}");
                ViewBag.Products = new List<ProductDto>();
                ViewBag.Branches = new List<BranchDto>();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockDto stockDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(stockDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("stocks", content);
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
            return View(stockDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"stocks/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<StockDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    // Ürünleri yükle
                    var productsResponse = await _httpClient.GetAsync("products");
                    var products = new List<ProductDto>();
                    if (productsResponse.IsSuccessStatusCode)
                    {
                        var productsContent = await productsResponse.Content.ReadAsStringAsync();
                        var productsApiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductDto>>>(productsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        products = productsApiResponse?.Data ?? new List<ProductDto>();
                    }

                    // Şubeleri yükle
                    var branchesResponse = await _httpClient.GetAsync("branches");
                    var branches = new List<BranchDto>();
                    if (branchesResponse.IsSuccessStatusCode)
                    {
                        var branchesContent = await branchesResponse.Content.ReadAsStringAsync();
                        var branchesApiResponse = JsonSerializer.Deserialize<ApiResponse<List<BranchDto>>>(branchesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        branches = branchesApiResponse?.Data ?? new List<BranchDto>();
                    }

                    ViewBag.Products = products;
                    ViewBag.Branches = branches;
                    
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
        public async Task<IActionResult> Edit(int id, StockDto stockDto)
        {
            if (id != stockDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(stockDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"stocks/{id}", content);
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
            return View(stockDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"stocks/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<StockDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                var response = await _httpClient.DeleteAsync($"stocks/{id}");
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