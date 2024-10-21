using WhoIsHome.DataAccess;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.AuthTokens;

public class RefreshTokenService(WhoIsHomeContext context, IUserContext userContext)
{
    public RefreshToken? GetForUser()
    {
        var refreshTokenModel = context.RefreshTokens
            .Where(t => t.UserId == userContext.UserId)
            .MaxBy(t => t.Issued);

        return refreshTokenModel?.ToRefreshToken();
    }
    
    public async Task<RefreshToken> CreateTokenAsync(CancellationToken cancellationToken)
    {
        var token = RefreshToken.Create(userContext.UserId);
        var model = token.ToModel();

        var dbToken = await context.RefreshTokens.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return dbToken.Entity.ToRefreshToken();
    }
}