using System.Globalization;

namespace WhoIsHome.WebApi.PushUp;

public class PushUpSettings
{
    public string? Token { get; set; }
    
    public bool? Enable { get; set; }

    public CultureInfo? LanguageCode { get; set; }
}