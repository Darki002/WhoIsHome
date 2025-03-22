using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoIsHome.External;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.AuthTokens;

public class RefreshTokenService(IDbContextFactory<WhoIsHomeContext> contextFactory, IDateTimeProvider dateTimeProvider, ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    public async Task<RefreshToken> CreateTokenAsync(int userId, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        
        RefreshToken token;
        bool tokenExists;
        
        do
        {
            token = RefreshToken.Create(userId, dateTimeProvider);
            tokenExists = await context.RefreshTokens
                .AsNoTracking()
                .AnyAsync(t => t.Token == token.Token, cancellationToken: cancellationToken);
        } while (tokenExists);

        var model = token.ToModel();
        var dbToken = await context.RefreshTokens.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("New Refresh Token was Generated for User {Id}", userId);
        
        return dbToken.Entity.ToRefreshToken(dateTimeProvider);
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
            newRefreshToken = RefreshToken.Create(token.UserId, dateTimeProvider);
            tokenExists = await context.RefreshTokens
                .AsNoTracking()
                .AnyAsync(t => t.Token == newRefreshToken.Token, cancellationToken: cancellationToken);
        }

        context.RefreshTokens.Update(token.ToModel());
        var dbToken = await context.RefreshTokens.AddAsync(newRefreshToken.ToModel(), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return dbToken.Entity.ToRefreshToken(dateTimeProvider);
    }

    public async Task LogOutAsync(int userId, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var tokens = await context.RefreshTokens
            .Where(t => t.UserId == userId)
            .Where(t => t.ExpiredAt >= dateTimeProvider.Now)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.ExpiredAt = dateTimeProvider.Now;
        }
        context.RefreshTokens.UpdateRange(tokens);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<RefreshToken> GetValidRefreshToken(string tokenToCheck, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var model = await context.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Token == tokenToCheck, cancellationToken);
        
        if (model is null)
        {
            throw new InvalidRefreshTokenException("No Token was found", null);
        }
        
        var token = model.ToRefreshToken(dateTimeProvider);

        if (token.IsValid() is false)
        {
            throw new InvalidRefreshTokenException("Token is invalid", token.ExpiredAt);
        }

        return token;
    }
}

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateTokenAsync(int userId, CancellationToken cancellationToken);
    Task<RefreshToken> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task LogOutAsync(int userId, CancellationToken cancellationToken);
}