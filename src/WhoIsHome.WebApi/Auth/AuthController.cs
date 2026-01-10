using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhoIsHome.AuthTokens;
using WhoIsHome.Entities;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Auth;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(
    IUserService userService, 
    JwtTokenService jwtTokenService,
    IPasswordHasher<User> passwordHasher,
    IUserContextProvider userContextProvider,
    ILogger<AuthController> logger) : Controller
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<LoginResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        
        var user = await userService.GetUserByEmailAsync(loginDto.Email, cancellationToken);
        if (user == null)
        {
            logger.LogInformation("Login Attempt failed, since no user was found for {email} from IP {IP} | UserAgent: {UserAgent}", loginDto.Email, ip, userAgent);
            return Unauthorized();
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            logger.LogInformation("Login Attempt failed because the email or password was incorrect from IP {IP} | UserAgent: {UserAgent}", ip, userAgent);
            return Unauthorized();
        }

        logger.LogInformation("New Login for {UserName} with ID {Id} from IP {IP} | UserAgent: {UserAgent}", user.UserName, user.Id, ip, userAgent);
        
        var token = await jwtTokenService.GenerateTokenAsync(user, cancellationToken);
        return Ok(new LoginResult(token.JwtToken!, token.RefreshToken!));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<RegisterResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        
        var result = await userService.CreateUserAsync(
            registerDto.UserName,
            registerDto.Email,
            registerDto.Password,
            cancellationToken);

        if (result.HasErrors)
        {
            return BadRequest(new ErrorResponse { Errors = result.ValidationErrors.Select(e => e.Message) });
        }
        
        logger.LogInformation("New registration {UserName} with ID {Id} from IP {IP} | UserAgent: {UserAgent}", result.Result.UserName, result.Result.Id, ip, userAgent);

            
        return Ok(new RegisterResult(result.Result.Id));
    }

    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType<RefreshResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromHeader(Name = "RefreshToken")] string refreshToken, CancellationToken cancellationToken)
    {
        var result = await jwtTokenService.RefreshTokenAsync(refreshToken, cancellationToken);

        if (result.HasError)
        {
            logger.LogInformation("Refresh Token is Invalid. | Reason: {Message}", result.Error);
            return Unauthorized();
        }
        
        return Ok(new RefreshResult(result.JwtToken!, result.RefreshToken!));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await jwtTokenService.LogOutAsync(userContextProvider.UserId, cancellationToken);
        return Ok();
    }

    // ReSharper disable NotAccessedPositionalProperty.Local
    private record LoginResult(string JwtToken, string RefreshToken);
    private record RegisterResult(int Id);
    private record RefreshResult(string JwtToken, string RefreshToken);
    // ReSharper restore NotAccessedPositionalProperty.Local
}