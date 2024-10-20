namespace WhoIsHome.AuthTokens;

public class RefreshToken(int? id, string token, DateTime? issued)
{
    private const int ExpiresInDays = 12;

    public int? Id { get; private init; } = id;
    
    public string Token { get; private init; } = token;

    public DateTime Issued { get; private set; } = issued ?? DateTime.Now;

    public bool IsValid => GetExpireDate() <= DateTime.Now;

    public DateTime GetExpireDate()
    {
        return Issued.AddDays(ExpiresInDays);
    }
}