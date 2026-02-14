using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Infrastructure.Persistence;

namespace PulseMonitor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public HealthController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            await _db.Database.CanConnectAsync();
            return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }
        catch
        {
            return StatusCode(503, new { status = "Unhealthy", timestamp = DateTime.UtcNow });
        }
    }
}
