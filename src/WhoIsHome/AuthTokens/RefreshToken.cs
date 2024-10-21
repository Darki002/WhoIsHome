namespace WhoIsHome.AuthTokens;

public class RefreshToken(int? id, int userId, string token, DateTime issued, DateTime? expiredAt)
{
    private const int ExpiresInDays = 12;

    public int? Id { get; private init; } = id;

    public int UserId { get; private init; } = userId;
    
    public string Token { get; private init; } = token;

    public DateTime Issued { get; private set; } = issued;

    public DateTime? ExpiredAt { get; set; } = expiredAt;

    public bool IsValid => GetExpireDate() <= DateTime.Now;

    public bool Validate(int requestUser)
    {
        return requestUser == UserId && IsValid;
    }
    
    public DateTime GetExpireDate()
    {
        return Issued.AddDays(ExpiresInDays);
    }

    public static RefreshToken Create(int userId)
    {
        var token = GenerateToken();
        return new RefreshToken(null, userId, token, DateTime.Now, null);
    }

    private static string GenerateToken()
    {
        throw new NotImplementedException();
    }

    public void Refresh()
    {
        ExpiredAt = DateTime.Now;
    }
}