using Aris.Application.DTOs.Auth;
using Aris.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aris.api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Email, request.Senha, cancellationToken);
        return Ok(result);
    }
}
