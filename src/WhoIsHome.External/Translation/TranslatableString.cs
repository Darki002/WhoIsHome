using System.Globalization;

namespace WhoIsHome.External.Translation;

public readonly record struct TranslatableString(string Value, params object?[] Args)
{
    public static implicit operator TranslatableString(string str) => new(str);

    public string Translate(ITranslationService translation, CultureInfo culture)
    {
        return translation.Translate(this, culture);
    }
}