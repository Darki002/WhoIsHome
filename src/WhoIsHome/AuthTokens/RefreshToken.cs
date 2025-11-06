using System.Security.Cryptography;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.AuthTokens;

public class RefreshToken(int? id, int userId, string token, DateTime issued, DateTime expiredAt)
{
    private const int ExpiresInDays = 14;

    private const int RefreshTokenLength = 64;

    public int? Id { get; } = id;

    public int UserId { get; } = userId;
    
    public string Token { get; } = token;

    public DateTime Issued { get; } = issued;

    public DateTime ExpiredAt { get; private set; } = expiredAt;

    public static RefreshToken Create(int userId, DateTime currentTime)
    {
        var token = GenerateToken();
        var issues = currentTime;
        var expiresAt = currentTime.AddDays(ExpiresInDays);
        return new RefreshToken(null, userId, token, issues, expiresAt);
    }
    
    private static string GenerateToken()
    {
        var randomNumber = new byte[RefreshTokenLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}