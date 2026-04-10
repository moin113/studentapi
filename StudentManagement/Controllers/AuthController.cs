using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentManagement.Application.DTOs.Request;
using StudentManagement.Application.DTOs.Response;
using StudentManagement.Application.Interfaces.Services;

namespace StudentManagement.Controllers;

/// <summary>
/// Controller for authentication and authorization.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns access/refresh tokens.
    /// Sample credentials: admin@test.com / AdminPassword123
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>Authentication tokens.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email {Email}.", request.Email);
        var result = await _authService.LoginAsync(request);
        return result.Success ? Ok(result.Data) : Unauthorized(result);
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">Registration details.</param>
    /// <returns>A success indicator.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registering new user with email {Email}.", request.Email);
        var result = await _authService.RegisterAsync(request);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token.
    /// </summary>
    /// <param name="request">The current refresh token.</param>
    /// <returns>New authentication tokens.</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        _logger.LogInformation("Refreshing token.");
        var result = await _authService.RefreshTokenAsync(request);
        return result.Success ? Ok(result.Data) : Unauthorized(result);
    }

    /// <summary>
    /// Logs out a user by invalidating their refresh token.
    /// </summary>
    /// <param name="request">The refresh token to invalidate.</param>
    /// <returns>A success indicator.</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        _logger.LogInformation("Logging out.");
        var result = await _authService.LogoutAsync(request.RefreshToken);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
}
