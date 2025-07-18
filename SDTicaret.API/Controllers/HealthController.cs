using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SDTicaret.API.Models;

namespace SDTicaret.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetHealth()
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();
        
        var isHealthy = healthReport.Status == HealthStatus.Healthy;
        
        var response = new
        {
            status = healthReport.Status.ToString(),
            checks = healthReport.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration
            }),
            totalDuration = healthReport.TotalDuration
        };

        if (isHealthy)
        {
            return Ok(ApiResponse<object>.SuccessResult(response, "Sistem sağlıklı"));
        }
        else
        {
            return StatusCode(503, ApiResponse<object>.ErrorResult("Sistem sağlıksız", new List<string> { healthReport.Status.ToString() }));
        }
    }
} 