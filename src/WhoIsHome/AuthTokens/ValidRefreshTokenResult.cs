namespace WhoIsHome.AuthTokens;

public record ValidRefreshTokenResult(RefreshToken? Token, string? Error)
{
    public RefreshToken Value => Token!;

    public bool HasError => Error is not null;
}