﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhoIsHome.Aggregates;
using WhoIsHome.AuthTokens;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.WebApi.Auth;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController(
    IUserAggregateService userAggregateService, 
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
        
        var user = await userAggregateService.GetUserByEmailAsync(loginDto.Email, cancellationToken);
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
        
        try
        {
            var user = await userAggregateService.CreateUserAsync(
                registerDto.UserName,
                registerDto.Email,
                registerDto.Password,
                cancellationToken);

            logger.LogInformation("New registration {UserName} with ID {Id} from IP {IP} | UserAgent: {UserAgent}", user.UserName, user.Id, ip, userAgent);

            
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