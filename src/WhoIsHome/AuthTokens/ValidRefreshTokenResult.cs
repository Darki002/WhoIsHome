using WhoIsHome.External.Models;

namespace WhoIsHome.AuthTokens;

public record ValidRefreshTokenResult(RefreshTokenModel? Token, string? Error)
{
    public RefreshTokenModel Value => Token!;

    public bool HasError => Error is not null;
}