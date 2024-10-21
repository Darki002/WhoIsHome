using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WhoIsHome.AuthTokens;

public class JwtTokenService(IConfiguration configuration, RefreshTokenService refreshTokenService, ILogger<JwtTokenService> logger)
{
    public async Task<AuthToken> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenService.CreateTokenAsync(user, cancellationToken);
        var jwtToken = GenerateJwtToken(user);
        return new AuthToken(jwtToken, refreshToken.Token);
    }

    public async Task<AuthToken> RefreshTokenAsync(User user, string token, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenService.GetValidRefreshToken(token, user.Id!.Value, cancellationToken);
        var newRefreshToken = refreshToken.Refresh();
        var jwtToken = GenerateJwtToken(user);
        
        return new AuthToken(jwtToken, newRefreshToken.Token);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        
        var secretKey = EnvironmentHelper.GetVariable(EnvVariables.JwtSecretKey);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", "User")
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
}