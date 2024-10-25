using System.Security.Cryptography;

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

    public bool IsValid()
    {
        return ExpiredAt >= DateTime.Now;
    }

    public static RefreshToken Create(int userId)
    {
        var token = GenerateToken();
        var issues = DateTime.Now;
        var expiresAt = DateTime.Now.AddDays(ExpiresInDays);
        return new RefreshToken(null, userId, token, issues, expiresAt);
    }

    public RefreshToken Refresh()
    {
        ExpiredAt = DateTime.Now;
        return Create(UserId);
    }
    
    private static string GenerateToken()
    {
        var randomNumber = new byte[RefreshTokenLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}