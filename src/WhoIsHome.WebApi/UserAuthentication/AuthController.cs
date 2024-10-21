using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.AuthTokens;
using WhoIsHome.Services;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.WebApi.UserAuthentication;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(
    UserAggregateService userAggregateService, 
    JwtTokenService jwtTokenService,
    IPasswordHasher<User> passwordHasher) : Controller
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var user = await userAggregateService.GetUserByEmailAsync(loginDto.Email, cancellationToken);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = await jwtTokenService.GenerateTokenAsync(user, cancellationToken);
        return Ok(new { token.JwtToken, token.RefreshToken });
    }

    [HttpPost("Register")]
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

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(RefreshDto refreshDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userAggregateService.GetUserByEmailAsync(refreshDto.Email, cancellationToken);
            
            if (user == null)
            {
                return Unauthorized("Invalid email.");
            }
            
            var token = await jwtTokenService.RefreshTokenAsync(user, refreshDto.RefreshToken, cancellationToken);
            return Ok(new { token.JwtToken, token.RefreshToken });
        }
        catch (InvalidRefreshTokenException)
        {
            return Unauthorized("Refresh Token is expired.");
        }
    }
}