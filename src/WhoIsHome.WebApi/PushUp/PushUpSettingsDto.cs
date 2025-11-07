namespace WhoIsHome.WebApi.PushUp;

public class PushUpSettingsDto
{
    public string? Token { get; set; }
    
    public bool? Enable { get; set; }

    public string? LanguageCode { get; set; }
}