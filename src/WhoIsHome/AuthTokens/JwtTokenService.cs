using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.AuthTokens;

public class JwtTokenService(IConfiguration configuration, IRefreshTokenService refreshTokenService, IUserAggregateService userAggregateService, ILogger<JwtTokenService> logger)
{
    public async Task<AuthToken> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenService.CreateTokenAsync(user.Id!.Value, cancellationToken);
        var jwtToken = GenerateJwtToken(user);
        return new AuthToken(jwtToken, refreshToken.Token);
    }

    public async Task<AuthToken> RefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var newRefreshToken = await refreshTokenService.RefreshAsync(token, cancellationToken);
        var user = await userAggregateService.GetAsync(newRefreshToken.UserId, cancellationToken);
        var jwtToken = GenerateJwtToken(user);
        
        return new AuthToken(jwtToken, newRefreshToken.Token);
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
        
        logger.LogInformation("New Token for User {Id} was generated", user.Id);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task LogOutAsync(int userId, CancellationToken cancellationToken)
    {
        await refreshTokenService.LogOutAsync(userId, cancellationToken);
    }
}