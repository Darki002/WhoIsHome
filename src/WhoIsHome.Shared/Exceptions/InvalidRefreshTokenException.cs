namespace WhoIsHome.Shared.Exceptions;

public class InvalidRefreshTokenException(string message, DateTime? expiredAt) : Exception(message)
{
    public DateTime? ExpiredAt { get; } = expiredAt;
}