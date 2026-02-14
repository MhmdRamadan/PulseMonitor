using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseMonitor.Application.Features.AlertRules.Commands.CreateAlertRule;
using PulseMonitor.Application.Features.AlertRules.Queries.GetAlertRulesByServer;
using PulseMonitor.Domain.Enums;

namespace PulseMonitor.API.Controllers;

[ApiController]
[Route("api/v1/servers/{serverId:guid}/alert-rules")]
[Authorize]
public class AlertRulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlertRulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAlertRules(Guid serverId)
    {
        var result = await _mediator.Send(new GetAlertRulesByServerQuery(serverId));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAlertRule(Guid serverId, [FromBody] CreateAlertRuleRequest request)
    {
        var id = await _mediator.Send(new CreateAlertRuleCommand(serverId, request.MetricType, request.Operator, request.ThresholdValue, request.Severity));
        return CreatedAtAction(nameof(GetAlertRules), new { serverId }, new { id });
    }
}

public record CreateAlertRuleRequest(MetricType MetricType, string Operator, double ThresholdValue, AlertSeverity Severity);
