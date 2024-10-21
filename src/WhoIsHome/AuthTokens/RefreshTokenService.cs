using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.AuthTokens;

public class RefreshTokenService(WhoIsHomeContext context)
{
    public async Task<RefreshToken> GetValidRefreshToken(string tokenToCheck, int userId,
        CancellationToken cancellationToken)
    {
        var model = await context.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == tokenToCheck, cancellationToken);

        var token = model?.ToRefreshToken();

        if (token is null || token.Validate(userId) is false)
        {
            throw new InvalidRefreshTokenException();
        }

        return token;
    }

    public async Task<RefreshToken> CreateTokenAsync(User user, CancellationToken cancellationToken)
    {
        var token = RefreshToken.Create(user.Id!.Value);
        var model = token.ToModel();

        var dbToken = await context.RefreshTokens.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return dbToken.Entity.ToRefreshToken();
    }
}