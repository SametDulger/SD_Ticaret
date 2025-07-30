using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace SDTicaret.API.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
        _logger = Log.Logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Request logla
        context.Request.EnableBuffering();
        var requestBody = await new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
        context.Request.Body.Position = 0;
        _logger.Information("HTTP Request: {Method} {Path} {Query} | Body: {Body}", context.Request.Method, context.Request.Path, context.Request.QueryString, requestBody);

        // Response logla
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        _logger.Information("HTTP Response: {StatusCode} {Path} | Body: {Body}", context.Response.StatusCode, context.Request.Path, responseText);

        await responseBody.CopyToAsync(originalBodyStream);
    }
}