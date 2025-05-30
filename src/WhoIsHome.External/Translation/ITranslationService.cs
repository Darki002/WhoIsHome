using System.Globalization;

namespace WhoIsHome.External.Translation;

public interface ITranslationService
{
    string this[string key, CultureInfo culture] { get; }
    
    string this[TranslatableString str, CultureInfo culture] { get; }
    
    string Translate(string key, CultureInfo? culture, params object?[] args);

    string Translate(TranslatableString str, CultureInfo? culture);
}