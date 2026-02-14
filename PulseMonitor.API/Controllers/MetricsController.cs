using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseMonitor.Application.Features.Metrics.Queries.GetLatestMetric;
using PulseMonitor.Application.Features.Metrics.Queries.GetMetricsByServer;

namespace PulseMonitor.API.Controllers;

[ApiController]
[Route("api/v1/servers/{serverId:guid}/metrics")]
[Authorize]
public class MetricsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MetricsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMetrics(Guid serverId, [FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc, [FromQuery] int limit = 100)
    {
        var result = await _mediator.Send(new GetMetricsByServerQuery(serverId, fromUtc, toUtc, limit));
        return Ok(result);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest(Guid serverId)
    {
        var result = await _mediator.Send(new GetLatestMetricQuery(serverId));
        if (result == null) return NotFound();
        return Ok(result);
    }
}
