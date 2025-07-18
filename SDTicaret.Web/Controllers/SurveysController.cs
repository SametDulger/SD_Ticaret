using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text;
using System.Text.Json;

namespace SDTicaret.Web.Controllers
{
    public class SurveysController : Controller
    {
        private readonly HttpClient _httpClient;

        public SurveysController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("surveys");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<SurveyDto>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(apiResponse?.Data ?? new List<SurveyDto>());
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Hata: {ex.Message}");
            }
            return View(new List<SurveyDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"surveys/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<SurveyDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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

                ViewBag.Customers = customers;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Veri yükleme hatası: {ex.Message}");
                ViewBag.Customers = new List<CustomerDto>();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyDto surveyDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(surveyDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("surveys", content);
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
            return View(surveyDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"surveys/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<SurveyDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
        public async Task<IActionResult> Edit(int id, SurveyDto surveyDto)
        {
            if (id != surveyDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var json = JsonSerializer.Serialize(surveyDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"surveys/{id}", content);
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
            return View(surveyDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"surveys/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<SurveyDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                var response = await _httpClient.DeleteAsync($"surveys/{id}");
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