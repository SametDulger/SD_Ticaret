using Microsoft.AspNetCore.Http;

namespace SDTicaret.Web.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Session.GetString("AccessToken");
        var path = context.Request.Path.Value?.ToLower();

        // Sadece bu sayfalar authentication gerektirmez
        var publicPaths = new[]
        {
            "/auth/login",
            "/auth/register", 
            "/auth/forgotpassword",
            "/auth/resetpassword",
            "/home/index",
            "/home",
            "/",
            "/error"
        };

        // Static dosyalar için authentication gerekmez
        var isStaticFile = path?.StartsWith("/css/") == true || 
                          path?.StartsWith("/js/") == true || 
                          path?.StartsWith("/lib/") == true || 
                          path?.StartsWith("/favicon") == true;

        // Public sayfalar için authentication gerekmez (tam eşleşme)
        var isPublicPath = publicPaths.Any(publicPath => 
            path?.Equals(publicPath) == true);

        // Eğer kullanıcı giriş yapmamışsa ve public bir sayfa değilse
        if (string.IsNullOrEmpty(token) && !isPublicPath && !isStaticFile)
        {
            // Login sayfasına yönlendir
            context.Response.Redirect("/Auth/Login");
            return;
        }

        await _next(context);
    }
} 