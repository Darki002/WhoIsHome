using Microsoft.EntityFrameworkCore;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.External.Database;

namespace WhoIsHome.Host.Authentication;

public class UserContext(IDbContextFactory<WhoIsHomeContext> contextFactory) : IUserContext
{
    private AuthenticatedUser? authenticatedUserCache;

    public int UserId { get; internal set; }

    public async Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
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

    public bool IsUserPermitted(int permittedUserId) => UserId == permittedUserId;
}