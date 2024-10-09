using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Aggregates;

public class DinnerTime(PresenceType presenceType, TimeOnly? time = null)
{
    public PresenceType PresenceType { get; set; } = presenceType;

    public TimeOnly? Time { get; set; } = time;

    public bool IsAtHome => PresenceType != PresenceType.NotPresent;

    public static DinnerTime Create(PresenceType presenceType, TimeOnly? time)
    {
        return presenceType switch
        {
            PresenceType.Unknown => !time.HasValue ? CreateUnknown() : throw new InvalidModelException("Can't set Time for Type Unknown."),
            PresenceType.Default => time.HasValue ? CreateDefault(time.Value) : throw new InvalidModelException("Must provide a Time for Default Type."),
            PresenceType.Late => time.HasValue ? CreateLate(time.Value) : throw new InvalidModelException("Must provide a Time for Late Type."),
            PresenceType.NotPresent => !time.HasValue ? CreateNotPresent() : throw new InvalidModelException("Can't set Time for Type NotPresent."),
            _ => throw new ArgumentOutOfRangeException(nameof(presenceType), presenceType, null)
        };
    }
    
    public DinnerTime Update(PresenceType presenceType, TimeOnly? time)
    {
        return presenceType switch
        {
            PresenceType.Unknown => !time.HasValue ? CreateUnknown() : throw new InvalidModelException("Can't set Time for Type Unknown."),
            PresenceType.Default => time.HasValue ? CreateDefault(time.Value) : throw new InvalidModelException("Must provide a Time for Default Type."),
            PresenceType.Late => time.HasValue ? CreateLate(time.Value) : throw new InvalidModelException("Must provide a Time for Late Type."),
            PresenceType.NotPresent => !time.HasValue ? CreateNotPresent() : throw new InvalidModelException("Can't set Time for Type NotPresent."),
            _ => throw new ArgumentOutOfRangeException(nameof(presenceType), presenceType, null)
        };
    }
    
    private static DinnerTime CreateUnknown()
    {
        return new DinnerTime( PresenceType.Unknown);
    }

    private static DinnerTime CreateDefault(TimeOnly time)
    {
        return new DinnerTime( PresenceType.Default, time);
    }

    private static DinnerTime CreateLate(TimeOnly time)
    {
        return new DinnerTime( PresenceType.Late, time);
    }

    private static DinnerTime CreateNotPresent()
    {
        return new DinnerTime( PresenceType.NotPresent);
    }
}