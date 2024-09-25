using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;

namespace WhoIsHome.WebApi.UserAuthentication;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController(UserAggregateService userService, JwtTokenService jwtTokenService, IPasswordHasher<User> passwordHasher) : Controller
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByEmailAsync(loginDto.Email, cancellationToken);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = jwtTokenService.GenerateToken(user);
        return Ok(new { Token = token });
    }
}