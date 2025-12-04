using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ecommerce.API.Controllers;

public class AuthController : ApiController
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ISender sender,
        ILogger<AuthController> logger) : base(sender)
    {
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] Command.RegisterCommand body)
    {
        var result = await Sender.Send(body);
        if (result.IsFailure)
            return HandlerFailure(result);
        return Ok(result);
    }

    [HttpPost("otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendOTP([FromBody] Command.SendOTPCommand body)
    {
        var result = await Sender.Send(body);
        if (result.IsFailure)
            return HandlerFailure(result);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(
        [FromBody] Command.LoginCommand body,
        [FromHeader(Name = "User-Agent")] string userAgent,
        [FromServices] IHttpContextAccessor httpContextAccessor
        )
    {
        var ip = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        var command = body with
        {
            UserAgent = userAgent,
            Ip = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown"
        };
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] Command.RefreshTokenCommand body,
        [FromHeader(Name = "User-Agent")] string userAgent,
        [FromServices] IHttpContextAccessor httpContextAccessor
        )
    {
        var command = body with
        {
            UserAgent = userAgent,
            Ip = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown"
        };
        var result = await Sender.Send(command);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] Command.LogoutCommand body)
    {
        var result = await Sender.Send(body);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("google-link")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuthorizationUrl(
        [FromHeader(Name = "User-Agent")] string userAgent,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var ip = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
        var result = await Sender.Send(new Query.GoogleLink(userAgent, ip));

        return Ok(result);
    }

    [HttpGet("google/callback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state)
    {
        Console.WriteLine(code, state);
        return Redirect("URL_CHUYEN_HUONG");
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] Command.ForgotPassword body)
    {
        var result = await Sender.Send(body);

        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("2fa/setup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SetupTwoFactorAuth([FromRoute] int userId)
    {
        return Ok(userId);
    }

    [HttpPost("2fa/disable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DisableTwoFactorAuth([FromBody] Command.DisableTwoFactor body, [FromRoute] int userId)
    {
        Console.WriteLine(userId);
        return Ok(body);
    }
}

