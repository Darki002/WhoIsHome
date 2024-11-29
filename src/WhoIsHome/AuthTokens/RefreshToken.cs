using System.Security.Cryptography;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.AuthTokens;

public class RefreshToken(int? id, int userId, string token, DateTime issued, DateTime expiredAt, IDateTimeProvider dateTimeProvider)
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
        return ExpiredAt >= dateTimeProvider.Now;
    }

    public static RefreshToken Create(int userId, IDateTimeProvider dateTimeProvider)
    {
        var token = GenerateToken();
        var issues = dateTimeProvider.Now;
        var expiresAt = dateTimeProvider.Now.AddDays(ExpiresInDays);
        return new RefreshToken(null, userId, token, issues, expiresAt, dateTimeProvider);
    }

    public RefreshToken Refresh()
    {
        ExpiredAt = dateTimeProvider.Now;
        return Create(UserId, dateTimeProvider);
    }
    
    private static string GenerateToken()
    {
        var randomNumber = new byte[RefreshTokenLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}