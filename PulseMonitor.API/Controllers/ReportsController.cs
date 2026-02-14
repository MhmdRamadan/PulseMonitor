using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseMonitor.Application.Features.Reports.Commands.CreateReport;
using PulseMonitor.Application.Features.Reports.Queries.GetReportById;
using PulseMonitor.Application.Features.Reports.Queries.GetReports;

namespace PulseMonitor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetReports([FromQuery] Guid? serverId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetReportsQuery(serverId, pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReport(Guid id)
    {
        var result = await _mediator.Send(new GetReportByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request)
    {
        var result = await _mediator.Send(new CreateReportCommand(request.ServerId, request.FromUtc, request.ToUtc));
        if (!result.Succeeded) return BadRequest(result.Errors);
        return CreatedAtAction(nameof(GetReport), new { id = result.Data!.Id }, result.Data);
    }
}

public record CreateReportRequest(Guid ServerId, DateTime FromUtc, DateTime ToUtc);
