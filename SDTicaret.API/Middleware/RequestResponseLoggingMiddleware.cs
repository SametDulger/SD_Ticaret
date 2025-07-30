using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace SDTicaret.API.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Basit request log
        Log.Information("HTTP Request: {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context);

        // Basit response log
        Log.Information("HTTP Response: {StatusCode} {Path}", context.Response.StatusCode, context.Request.Path);
    }
}