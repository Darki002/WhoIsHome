namespace WhoIsHome.Shared.Types;

public enum PresenceType
{
    Unknown,
    Default,
    Late,
    NotPresent
}

public static class PresenceTypeHelper
{
    public static PresenceType FromString(string str)
    {
        return str switch
        {
            nameof(PresenceType.Unknown) => PresenceType.Unknown,
            nameof(PresenceType.Default) => PresenceType.Default,
            nameof(PresenceType.Late) => PresenceType.Late,
            nameof(PresenceType.NotPresent) => PresenceType.NotPresent,
            _ => throw new ArgumentOutOfRangeException(nameof(str), str, null)
        };
    }
}