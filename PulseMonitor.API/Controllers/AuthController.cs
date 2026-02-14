using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseMonitor.Application.Features.Users.Commands.Login;
using PulseMonitor.Application.Features.Users.Commands.Register;

namespace PulseMonitor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(new RegisterCommand(request.UserName, request.Email, request.Password));
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.UserName, request.Password));
        if (!result.Succeeded) return Unauthorized(result.Errors);
        return Ok(result.Data);
    }
}

public record RegisterRequest(string UserName, string Email, string Password);
public record LoginRequest(string UserName, string Password);
