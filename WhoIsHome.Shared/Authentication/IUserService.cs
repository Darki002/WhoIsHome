namespace WhoIsHome.Shared.Authentication;

public interface IUserService
{
    Task<AuthenticatedUser> GetCurrentUserAsync();
    
    Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken);
    
    bool IsUserPermitted(int permittedUserId);
}