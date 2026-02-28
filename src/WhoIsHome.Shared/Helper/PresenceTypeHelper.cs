using WhoIsHome.Shared.Types;

namespace WhoIsHome.Shared.Helper;

public static class PresenceTypeHelper
{
    public static bool IsDefined(string str)
    {
        return str switch
        {
            nameof(PresenceType.Unknown) => true,
            nameof(PresenceType.Default) => true,
            nameof(PresenceType.Late) => true,
            nameof(PresenceType.NotPresent) => true,
            _ => false
        };
    }
    
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