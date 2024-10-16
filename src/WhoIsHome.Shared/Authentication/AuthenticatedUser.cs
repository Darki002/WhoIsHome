namespace WhoIsHome.Shared.Authentication;

public record AuthenticatedUser
{
    public required int Id { get; init; }

    public required string UserName { get; init; }

    public required string Email { get; init; }

    public required string PasswordHash { get; init;  }
}