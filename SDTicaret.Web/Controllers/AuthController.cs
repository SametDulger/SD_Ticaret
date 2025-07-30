using Microsoft.AspNetCore.Mvc;
using SDTicaret.Web.Models;
using System.Text;
using System.Text.Json;

namespace SDTicaret.Web.Controllers;

public class AuthController : Controller
{
    private readonly HttpClient _httpClient;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
    {
        Console.WriteLine("=== LOGIN ACTION STARTED ===");
        Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
        
        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState is invalid:");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Error: {error.ErrorMessage}");
            }
            return View(loginDto);
        }

        Console.WriteLine($"LoginDto received - Username: '{loginDto?.Username}', Password length: {loginDto?.Password?.Length}");
        Console.WriteLine($"Request.Form keys: {string.Join(", ", Request.Form.Keys)}");
        Console.WriteLine($"Request.Form values: {string.Join(", ", Request.Form.Select(kv => $"{kv.Key}={kv.Value}"))}");
        
        if (loginDto == null)
        {
            Console.WriteLine("LoginDto is null!");
            ModelState.AddModelError("", "Form verileri alınamadı");
            return View();
        }

        try
        {
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"Login attempt for user: {loginDto.Username}");
            Console.WriteLine($"Request URL: {_httpClient.BaseAddress}auth/login");
            Console.WriteLine($"Request JSON: {json}");
            Console.WriteLine($"Content-Type: {content.Headers.ContentType}");
            Console.WriteLine($"Content-Length: {content.Headers.ContentLength}");
            
            var response = await _httpClient.PostAsync("auth/login", content);
            
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"))}");
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");
            
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<TokenDto>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse?.Data != null)
                {
                    HttpContext.Session.SetString("AccessToken", apiResponse.Data.AccessToken);
                    HttpContext.Session.SetString("RefreshToken", apiResponse.Data.RefreshToken);
                    HttpContext.Session.SetString("Username", apiResponse.Data.Username);
                    HttpContext.Session.SetString("Role", apiResponse.Data.Role);

                    Console.WriteLine($"Login successful for user: {apiResponse.Data.Username}");
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    Console.WriteLine($"API response data is null. Message: {apiResponse?.Message}");
                    ModelState.AddModelError("", apiResponse?.Message ?? "Giriş başarısız");
                }
            }
            else
            {
                Console.WriteLine($"Login failed with status: {response.StatusCode}");
                ModelState.AddModelError("", $"Giriş başarısız: {response.StatusCode}");
            }

            return View(loginDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login exception: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            ModelState.AddModelError("", $"Giriş yapılırken bir hata oluştu: {ex.Message}");
            return View(loginDto);
        }
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return View(registerDto);

        try
        {
            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/register", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kayıt başarıyla tamamlandı. Giriş yapabilirsiniz.";
                return RedirectToAction(nameof(Login));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
            
            if (errorResponse.TryGetProperty("message", out var message))
            {
                ModelState.AddModelError("", message.GetString() ?? "Kayıt sırasında bir hata oluştu");
            }
            else
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu");
            }

            return View(registerDto);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu");
            return View(registerDto);
        }
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Auth");
    }

    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Login));

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("auth/me");
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var userDto = JsonSerializer.Deserialize<UserDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(userDto);
            }
        }
        catch (Exception)
        {
            // Hata durumunda login sayfasına yönlendir
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpPost("profile")]
    public async Task<IActionResult> Profile(UserDto userDto)
    {
        if (!ModelState.IsValid)
            return View(userDto);

        var token = HttpContext.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Login));

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var json = JsonSerializer.Serialize(userDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("auth/profile", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Profil başarıyla güncellendi";
                return RedirectToAction(nameof(Profile));
            }

            ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu");
            return View(userDto);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu");
            return View(userDto);
        }
    }

    [HttpGet("change-password")]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        if (!ModelState.IsValid)
            return View(changePasswordDto);

        var token = HttpContext.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToAction(nameof(Login));

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var json = JsonSerializer.Serialize(changePasswordDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/change-password", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi";
                return RedirectToAction(nameof(Profile));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
            
            if (errorResponse.TryGetProperty("message", out var message))
            {
                ModelState.AddModelError("", message.GetString() ?? "Şifre değiştirilirken bir hata oluştu");
            }
            else
            {
                ModelState.AddModelError("", "Şifre değiştirilirken bir hata oluştu");
            }

            return View(changePasswordDto);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Şifre değiştirilirken bir hata oluştu");
            return View(changePasswordDto);
        }
    }

    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        if (!ModelState.IsValid)
            return View(forgotPasswordDto);

        try
        {
            var json = JsonSerializer.Serialize(forgotPasswordDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/forgot-password", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi";
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError("", "Şifre sıfırlama işlemi sırasında bir hata oluştu");
            return View(forgotPasswordDto);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Şifre sıfırlama işlemi sırasında bir hata oluştu");
            return View(forgotPasswordDto);
        }
    }

    [HttpGet("reset-password")]
    public IActionResult ResetPassword([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Geçersiz token";
            return RedirectToAction(nameof(Login));
        }

        var resetPasswordDto = new ResetPasswordDto { Token = token };
        return View(resetPasswordDto);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        if (!ModelState.IsValid)
            return View(resetPasswordDto);

        try
        {
            var json = JsonSerializer.Serialize(resetPasswordDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/reset-password", content);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla sıfırlandı. Yeni şifrenizle giriş yapabilirsiniz.";
                return RedirectToAction(nameof(Login));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
            
            if (errorResponse.TryGetProperty("message", out var message))
            {
                ModelState.AddModelError("", message.GetString() ?? "Şifre sıfırlanırken bir hata oluştu");
            }
            else
            {
                ModelState.AddModelError("", "Şifre sıfırlanırken bir hata oluştu");
            }

            return View(resetPasswordDto);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Şifre sıfırlanırken bir hata oluştu");
            return View(resetPasswordDto);
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        var role = HttpContext.Session.GetString("Role");
        
        if (string.IsNullOrEmpty(token) || role != "Admin")
            return RedirectToAction("Login", "Auth");

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("users");
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<UserDto>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(users ?? new List<UserDto>());
            }
        }
        catch (Exception)
        {
            // Hata durumunda boş liste göster
        }

        return View(new List<UserDto>());
    }

    [HttpPost("users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(int id)
    {
        var token = HttpContext.Session.GetString("AccessToken");
        var role = HttpContext.Session.GetString("Role");
        
        if (string.IsNullOrEmpty(token) || role != "Admin")
            return RedirectToAction("Login", "Auth");

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsync($"users/{id}/activate", null);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla aktifleştirildi";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı aktifleştirilirken bir hata oluştu";
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Kullanıcı aktifleştirilirken bir hata oluştu";
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpPost("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var token = HttpContext.Session.GetString("AccessToken");
        var role = HttpContext.Session.GetString("Role");
        
        if (string.IsNullOrEmpty(token) || role != "Admin")
            return RedirectToAction("Login", "Auth");

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsync($"users/{id}/deactivate", null);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla deaktifleştirildi";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı deaktifleştirilirken bir hata oluştu";
            }
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Kullanıcı deaktifleştirilirken bir hata oluştu";
        }

        return RedirectToAction(nameof(Users));
    }
} 