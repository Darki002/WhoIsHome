using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoIsHome.External;
using WhoIsHome.External.Models;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.AuthTokens;

public class RefreshTokenService(IDbContextFactory<WhoIsHomeContext> contextFactory, IDateTimeProvider dateTimeProvider, ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    public async Task<RefreshTokenModel> CreateTokenAsync(int userId, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        
        RefreshToken token;
        bool tokenExists;
        
        do
        {
            token = RefreshToken.Create(userId, dateTimeProvider.Now);
            tokenExists = await context.RefreshTokens
                .AsNoTracking()
                .AnyAsync(t => t.Token == token.Token, cancellationToken: cancellationToken);
        } while (tokenExists);

        var model = new RefreshTokenModel
        {
            Token = token.Token,
            Issued = token.Issued,
            ExpiredAt = token.ExpiredAt,
            UserId = token.UserId
        };
        
        var dbToken = await context.RefreshTokens.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("New Refresh Token was Generated for User {Id}", userId);
        
        return dbToken.Entity;
    }

    public async Task<ValidRefreshTokenResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var result = await GetValidRefreshToken(refreshToken, cancellationToken);

        if (result.HasError)
        {
            return result;
        }
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        result.Value.ExpiredAt = dateTimeProvider.Now;
        var newRefreshToken = await CreateTokenAsync(result.Value.UserId, cancellationToken);

        context.RefreshTokens.Update(result.Value);
        var dbToken = await context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return new ValidRefreshTokenResult(dbToken.Entity, null);
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

    private async Task<ValidRefreshTokenResult> GetValidRefreshToken(string tokenToCheck, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var token = await context.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Token == tokenToCheck, cancellationToken);
        
        if (token is null)
        {
            return new ValidRefreshTokenResult(null, "No Token was found");
        }

        if (token.ExpiredAt < dateTimeProvider.Now)
        {
            logger.LogInformation("Refresh Token is Invalid. ExpiredAt: {ExpiredAt}", token.ExpiredAt);
            return new ValidRefreshTokenResult(null, "Token is invalid");
        }

        return new ValidRefreshTokenResult(token, null);
    }
}