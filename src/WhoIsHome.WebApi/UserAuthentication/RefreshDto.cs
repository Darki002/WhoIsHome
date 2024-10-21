namespace WhoIsHome.WebApi.UserAuthentication;

public class RefreshDto
{
    public string Email { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}