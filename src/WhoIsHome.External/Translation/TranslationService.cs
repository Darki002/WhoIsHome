using System.Globalization;

namespace WhoIsHome.External.Translation;

internal class TranslationService : ITranslationService
{
    public string this[string key, CultureInfo culture] => Resource.ResourceManager.GetString(key, culture)
                                                           ?? throw new ArgumentException("Resource does not exist",
                                                               nameof(key));

    public string this[TranslatableString str, CultureInfo culture] => Translate(str.Value, culture, str.Args);

    public string Translate(string key, CultureInfo culture, params object?[] args)
    {
        var template = this[key, culture];
        return args.Length > 0
            ? string.Format(template, args)
            : template;
    }

    public string Translate(TranslatableString str, CultureInfo culture)
    {
        return Translate(str.Value, culture, str.Args);
    }
}