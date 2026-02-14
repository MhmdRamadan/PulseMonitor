using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseMonitor.Application.Features.Servers.Commands.CreateServer;
using PulseMonitor.Application.Features.Servers.Commands.DeleteServer;
using PulseMonitor.Application.Features.Servers.Commands.UpdateServer;
using PulseMonitor.Application.Features.Servers.Queries.GetServerById;
using PulseMonitor.Application.Features.Servers.Queries.GetServers;

namespace PulseMonitor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ServersController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetServers([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] string? sortBy = "Name", [FromQuery] bool sortDescending = false)
    {
        var result = await _mediator.Send(new GetServersQuery(search, pageNumber, pageSize, sortBy, sortDescending));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetServer(Guid id)
    {
        var result = await _mediator.Send(new GetServerByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateServer([FromBody] CreateServerRequest request)
    {
        var id = await _mediator.Send(new CreateServerCommand(request.Name, request.HostName, request.IpAddress, request.Description));
        return CreatedAtAction(nameof(GetServer), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateServer(Guid id, [FromBody] UpdateServerRequest request)
    {
        await _mediator.Send(new UpdateServerCommand(id, request.Name, request.HostName, request.IpAddress, request.Description));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteServer(Guid id)
    {
        await _mediator.Send(new DeleteServerCommand(id));
        return NoContent();
    }
}

public record CreateServerRequest(string Name, string HostName, string? IpAddress, string? Description);
public record UpdateServerRequest(string Name, string HostName, string? IpAddress, string? Description);
