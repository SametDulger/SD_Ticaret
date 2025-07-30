using System.Net;
using System.Text.Json;
using FluentValidation;

namespace SDTicaret.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var endpoint = context.Request.Path;
            var method = context.Request.Method;
            var query = context.Request.QueryString.ToString();
            var user = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Anonymous";

            if (ex is ValidationException validationEx)
            {
                _logger.LogWarning(ex, "Validation error at {Endpoint} [{Method}] by {User} | Query: {Query} | Errors: {Errors}", endpoint, method, user, query, validationEx.Errors.Select(e => e.ErrorMessage));
            }
            else if (ex is UnauthorizedAccessException)
            {
                _logger.LogWarning(ex, "Unauthorized access at {Endpoint} [{Method}] by {User} | Query: {Query}", endpoint, method, user, query);
            }
            else
            {
                _logger.LogError(ex, "Exception at {Endpoint} [{Method}] by {User} | Query: {Query}", endpoint, method, user, query);
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        object response;

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = "Doğrulama hatası oluştu.",
                        details = validationEx.Errors.Select(e => e.ErrorMessage),
                        timestamp = DateTime.UtcNow
                    }
                };
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.Message,
                        timestamp = DateTime.UtcNow
                    }
                };
                break;
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.Message,
                        timestamp = DateTime.UtcNow
                    }
                };
                break;
            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = exception.Message,
                        details = exception.Message,
                        timestamp = DateTime.UtcNow
                    }
                };
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    error = new
                    {
                        message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.",
                        details = exception.Message,
                        timestamp = DateTime.UtcNow
                    }
                };
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
} 