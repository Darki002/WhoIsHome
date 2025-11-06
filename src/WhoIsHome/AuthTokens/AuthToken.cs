namespace WhoIsHome.AuthTokens;

public record AuthToken(string? JwtToken, string? RefreshToken, string? Error = null)
{
    public AuthToken(string error) : this(null, null, error) { }

    public bool HasError => Error is not null;
}