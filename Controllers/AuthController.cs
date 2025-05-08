using BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AuthService _authService;

    public AuthController(IConfiguration config, AuthService authService)
    {
        _config = config;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = string.Empty;
        try
        {
            token = await _authService.AuthenticateUser(request.Login, request.Password);
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException || ex is InvalidCredentialException)
                return Unauthorized("Invalid login or password!");
        }

        return Ok(new { Token = token });
    }
}

public record LoginRequest(string Login, string Password);