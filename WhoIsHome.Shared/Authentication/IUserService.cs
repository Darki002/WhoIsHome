namespace WhoIsHome.Shared.Authentication;

public interface IUserService
{
    int UserId { get; }
    
    Task<AuthenticatedUser> GetCurrentUserAsync();
    
    Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken);
    
    bool IsUserPermitted(int permittedUserId);
}