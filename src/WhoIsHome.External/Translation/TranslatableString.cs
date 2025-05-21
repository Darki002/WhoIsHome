namespace WhoIsHome.External.Translation;

public readonly record struct TranslatableString(string Value, params object?[] Args)
{
    public static implicit operator TranslatableString(string str) => new(str);
}