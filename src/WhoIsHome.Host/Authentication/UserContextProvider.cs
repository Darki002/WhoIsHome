using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.External.Database;

namespace WhoIsHome.Host.Authentication;

internal class UserContextProvider : IUserContextProvider
{
    private readonly int? userId;
    private readonly WhoIsHomeContext dbContext;
    
    private AuthenticatedUser? authenticatedUserCache;
    
    public int UserId => userId ?? throw new InvalidOperationException("UserId was not initialized.");

    public UserContextProvider(IHttpContextAccessor httpContextAccessor, WhoIsHomeContext whoIsHomeDbContext)
    { 
        dbContext = whoIsHomeDbContext;
        
        var idString = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId = int.TryParse(idString, out var id) ? id : null;
    }
    
    public async Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (authenticatedUserCache is not null)
        {
            return authenticatedUserCache;
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == UserId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException($"User with id {UserId} not found.");
        }

        authenticatedUserCache = new AuthenticatedUser
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
        
        return authenticatedUserCache;
    }

    public bool IsUserPermitted(int permittedUserId) => UserId == permittedUserId;
}