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
    RefreshTokenService refreshTokenService,
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

        var refreshToken = await refreshTokenService.CreateTokenAsync(user, cancellationToken);
        var token = jwtTokenService.GenerateTokenAsync(user);
        return Ok(new { Token = token, RefreshToken = refreshToken });
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
            return BadRequest("Email is already in use!");
        }
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(RefreshDto refreshDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userAggregateService.GetUserByEmailAsync(refreshDto.Email, cancellationToken);

            if (user is null)
            {
                return BadRequest($"User with the email {refreshDto.Email} does not exist.");
            }
            
            var isValid = await refreshTokenService.GetValidRefreshToken(
                refreshDto.RefreshToken, 
                user.Id!.Value, 
                cancellationToken);

            if (isValid is false)
            {
                return BadRequest("Refresh Token does not exist.");
            }
            
            var token = jwtTokenService.GenerateTokenAsync(user);
            return Ok(new { Token = token });
        }
        catch (InvalidRefreshTokenException)
        {
            return Unauthorized("Refresh Token is expired.");
        }
    }
}