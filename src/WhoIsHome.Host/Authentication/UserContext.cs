using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.Shared.Authentication;
using System.Security.Claims;

namespace WhoIsHome.Host.Authentication;

// TODO: move to middleware
public class UserContext(
    IHttpContextAccessor httpContextAccessor,
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    ILogger<UserContext> logger) : IUserContext
{
    private AuthenticatedUser? authenticatedUserCache;

    private int? idCache;

    public int UserId => GetUserId() ?? throw new InvalidClaimsException("No User is present in the Request.");

    public async Task<AuthenticatedUser> GetCurrentUserAsync()
    {
        return await GetCurrentUserAsync(CancellationToken.None);
    }

    public async Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        if (GetUserId() is null)
        {
            logger.LogInformation("(Invalid Claims) | No User ID found in request");
            throw new InvalidClaimsException("No User is present in the Request.");
        }

        if (authenticatedUserCache is not null)
        {
            return authenticatedUserCache;
        }

        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users.AsNoTracking().SingleAsync(u => u.Id == UserId, cancellationToken);

        authenticatedUserCache = new AuthenticatedUser
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PasswordHash = user.Password
        };
        
        return authenticatedUserCache;
    }

    public bool IsUserPermitted(int permittedUserId) => GetUserId() == permittedUserId;

    private int? GetUserId() => idCache ?? GetIdFromClaims();

    private int? GetIdFromClaims()
    {
        var idString = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        idCache = idString is not null ? int.Parse(idString) : null;
        return idCache;
    }
}