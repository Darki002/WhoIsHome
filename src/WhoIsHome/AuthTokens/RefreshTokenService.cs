using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoIsHome.DataAccess;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.AuthTokens;

public class RefreshTokenService(IDbContextFactory<WhoIsHomeContext> contextFactory, ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    public async Task<RefreshToken> CreateTokenAsync(int userId, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        
        RefreshToken token;
        bool tokenExists;
        
        do
        {
            token = RefreshToken.Create(userId);
            tokenExists = await context.RefreshTokens
                .AsNoTracking()
                .AnyAsync(t => t.Token == token.Token, cancellationToken: cancellationToken);
        } while (tokenExists);

        var model = token.ToModel();
        var dbToken = await context.RefreshTokens.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("New Refresh Token was Generated for User {Id}", userId);
        
        return dbToken.Entity.ToRefreshToken();
    }

    public async Task<RefreshToken> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await GetValidRefreshToken(refreshToken, cancellationToken);
        
        var newRefreshToken = token.Refresh();

        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var ka = context.RefreshTokens.AsNoTracking().ToList();
        ka.ForEach(t => Console.WriteLine(t.Token));
        
        var tokenExists = await context.RefreshTokens
            .AsNoTracking()
            .AnyAsync(t => t.Token == newRefreshToken.Token, cancellationToken: cancellationToken);
        
        while (tokenExists)
        {
            newRefreshToken = RefreshToken.Create(token.UserId);
            tokenExists = await context.RefreshTokens
                .AsNoTracking()
                .AnyAsync(t => t.Token == newRefreshToken.Token, cancellationToken: cancellationToken);
        }

        context.RefreshTokens.Update(token.ToModel());
        var dbToken = await context.RefreshTokens.AddAsync(newRefreshToken.ToModel(), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return dbToken.Entity.ToRefreshToken();
    }

    private async Task<RefreshToken> GetValidRefreshToken(string tokenToCheck, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var model = await context.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Token == tokenToCheck, cancellationToken);
        var token = model?.ToRefreshToken();
        if (token is null || token.IsValid() is false)
        {
            throw new InvalidRefreshTokenException();
        }

        return token;
    }
}

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateTokenAsync(int userId, CancellationToken cancellationToken);
    Task<RefreshToken> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
}