using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhoIsHome.Aggregates;
using WhoIsHome.AuthTokens;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Auth;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController(
    IUserAggregateService userAggregateService, 
    JwtTokenService jwtTokenService,
    IUserContext userContext,
    IPasswordHasher<User> passwordHasher,
    ILogger<AuthController> logger) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var user = await userAggregateService.GetUserByEmailAsync(loginDto.Email, cancellationToken);
        if (user == null)
        {
            logger.LogInformation("Login Attempt failed, since no user was found");
            return Unauthorized("Invalid email or password.");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            logger.LogInformation("Login Attempt failed because the email or password was incorrect");
            return Unauthorized("Invalid email or password.");
        }

        var token = await jwtTokenService.GenerateTokenAsync(user, cancellationToken);
        return Ok(new { token.JwtToken, token.RefreshToken });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userAggregateService.CreateUserAsync(
                registerDto.UserName,
                registerDto.Email,
                registerDto.Password,
                cancellationToken);

            return Ok(new { user.Id });
        }
        catch (EmailInUseException)
        {
            return BadRequest("Email is already in use.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromHeader(Name = "RefreshToken")] string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            var token = await jwtTokenService.RefreshTokenAsync(refreshToken, cancellationToken);
            return Ok(new { token.JwtToken, token.RefreshToken });
        }
        catch (InvalidRefreshTokenException)
        {
            return Unauthorized("Refresh Token is expired.");
        }
    }
}