using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WhoIsHome.Entities;
using WhoIsHome.Services;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.AuthTokens;

public class JwtTokenService(IConfiguration configuration, IRefreshTokenService refreshTokenService, IUserService userService, ILogger<JwtTokenService> logger)
{
    public async Task<AuthToken> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenService.CreateTokenAsync(user.Id, cancellationToken);
        var jwtToken = GenerateJwtToken(user);
        return new AuthToken(jwtToken, refreshToken.Token);
    }

    public async Task<AuthToken> RefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var result = await refreshTokenService.RefreshAsync(token, cancellationToken);

        if (result.HasError)
        {
            return new AuthToken(result.Error!);
        }
        
        var user = await userService.GetAsync(result.Value.UserId, cancellationToken);

        if (user is null)
        {
            return new AuthToken("No user found to generate Token for.");
        }
        
        var jwtToken = GenerateJwtToken(user);
        return new AuthToken(jwtToken, result.Value.Token);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var secretKey = configuration.GetJwtSecretKey();
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
            new Claim(ClaimTypes.Role, "User")
        };
        
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"]!)),
            signingCredentials: creds
        );
        
        logger.LogInformation("New Token for User {UserName} was generated", user.UserName);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task LogOutAsync(int userId, CancellationToken cancellationToken)
    {
        await refreshTokenService.LogOutAsync(userId, cancellationToken);
    }
}