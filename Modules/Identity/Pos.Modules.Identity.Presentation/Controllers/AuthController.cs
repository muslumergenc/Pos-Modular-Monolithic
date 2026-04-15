using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pos.Modules.Identity.Application.Commands.Login;
using Pos.Modules.Identity.Application.Commands.Register;
using Pos.Modules.Identity.Application.DTOs;

namespace Pos.Modules.Identity.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Yeni kullanıcı kaydı</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _mediator.Send(new RegisterCommand(dto.FullName, dto.Email, dto.Password, dto.ConfirmPassword));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>Kullanıcı girişi — JWT token döner</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _mediator.Send(new LoginCommand(dto.Email, dto.Password));
        if (!result.IsSuccess) return Unauthorized(new { error = result.Error });
        return Ok(result.Value);
    }
}

