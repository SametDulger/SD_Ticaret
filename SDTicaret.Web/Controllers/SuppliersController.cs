using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text;
using System.Text.Json;

namespace SDTicaret.Web.Controllers;

public class SuppliersController : Controller
{
    private readonly HttpClient _httpClient;

    public SuppliersController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync("suppliers");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<SupplierDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(apiResponse?.Data ?? new List<SupplierDto>());
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Hata: {ex.Message}");
        }
        return View(new List<SupplierDto>());
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"suppliers/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<SupplierDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(apiResponse?.Data);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Hata: {ex.Message}");
        }
        return NotFound();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SupplierDto supplier)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var json = JsonSerializer.Serialize(supplier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("suppliers", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
        }
        return View(supplier);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"suppliers/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<SupplierDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SupplierDto supplier)
    {
        if (id != supplier.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var json = JsonSerializer.Serialize(supplier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"suppliers/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
        }
        return View(supplier);
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"suppliers/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<SupplierDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(apiResponse?.Data);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Hata: {ex.Message}");
        }
        return NotFound();
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"suppliers/{id}");
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