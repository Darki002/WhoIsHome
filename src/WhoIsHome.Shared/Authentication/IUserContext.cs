namespace WhoIsHome.Shared.Authentication;

public interface IUserContext
{
    int UserId { get; }
    
    Task<AuthenticatedUser> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    
    bool IsUserPermitted(int permittedUserId);
}