namespace WhoIsHome.AuthTokens;

public record AuthToken(string JwtToken, string refreshToken);