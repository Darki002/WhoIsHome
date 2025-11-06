using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhoIsHome.AuthTokens;
using WhoIsHome.Entities;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.Auth;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController(
    IUserService userService, 
    JwtTokenService jwtTokenService,
    IPasswordHasher<User> passwordHasher,
    IUserContext userContext,
    ILogger<AuthController> logger) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        
        var user = await userService.GetUserByEmailAsync(loginDto.Email, cancellationToken);
        if (user == null)
        {
            logger.LogInformation("Login Attempt failed, since no user was found for {email} from IP {IP} | UserAgent: {UserAgent}", loginDto.Email, ip, userAgent);
            return Unauthorized("Invalid email or password.");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            logger.LogInformation("Login Attempt failed because the email or password was incorrect from IP {IP} | UserAgent: {UserAgent}", ip, userAgent);
            return Unauthorized("Invalid email or password.");
        }

        logger.LogInformation("New Login for {UserName} with ID {Id} from IP {IP} | UserAgent: {UserAgent}", user.UserName, user.Id, ip, userAgent);
        
        var token = await jwtTokenService.GenerateTokenAsync(user, cancellationToken);
        return Ok(new { token.JwtToken, token.RefreshToken });
    }

    [HttpPost]
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
            return BadRequest(result.ValidationErrors);
        }
        
        logger.LogInformation("New registration {UserName} with ID {Id} from IP {IP} | UserAgent: {UserAgent}", result.Value.UserName, result.Value.Id, ip, userAgent);

            
        return Ok(new { result.Value.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromHeader(Name = "RefreshToken")] string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            var token = await jwtTokenService.RefreshTokenAsync(refreshToken, cancellationToken);
            return Ok(new { token.JwtToken, token.RefreshToken });
        }
        catch (InvalidRefreshTokenException e)
        {
            logger.LogInformation("Refresh Token is Invalid. ExpiredAt: {ExpiredAt} | Reason: {Message}", e.ExpiredAt, e.Message);
            return Unauthorized("Refresh Token is Invalid.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await jwtTokenService.LogOutAsync(userContext.UserId, cancellationToken);
        return Ok();
    }
}