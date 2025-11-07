namespace WhoIsHome.AuthTokens;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateTokenAsync(int userId, CancellationToken cancellationToken);
    Task<ValidRefreshTokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task LogOutAsync(int userId, CancellationToken cancellationToken);
}