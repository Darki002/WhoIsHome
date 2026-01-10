using Microsoft.EntityFrameworkCore;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.External.Database;

namespace WhoIsHome.Host.Authentication;

internal class UserContextProvider(UserContextInfo userContextInfo, WhoIsHomeContext context) : IUserContextProvider
{
    private AuthenticatedUser? authenticatedUserCache;

    public int UserId => userContextInfo.UserId;

    public async Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (authenticatedUserCache is not null)
        {
            return authenticatedUserCache;
        }

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

internal class UserContextInfo
{
    private int? userId;
    public int UserId => userId ?? throw new InvalidOperationException("UserId was not initialized.");
    
    internal void Init(int id)
    {
        userId = id;
    }
}