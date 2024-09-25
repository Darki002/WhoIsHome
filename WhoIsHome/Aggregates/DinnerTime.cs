using WhoIsHome.Shared.BaseTypes;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Aggregates;

public class DinnerTime : AggregateBase
{
    public int? Id { get; private set; }

    public PresentsType PresentsType { get; set; }
    
    public TimeOnly? Time { get; set; }

    public bool IsAtHome => PresentsType != PresentsType.NotPresent;

    private DinnerTime(int? id, PresentsType presentsType, TimeOnly? time = null)
    {
        Id = id;
        PresentsType = presentsType;
        Time = time;
    }

    public static DinnerTime CreateUnknown()
    {
        return new DinnerTime(null, PresentsType.Unknown);
    }

    public static DinnerTime CreateDefault(TimeOnly time)
    {
        return new DinnerTime(null, PresentsType.Default, time);
    }
    
    public static DinnerTime CreateLate(TimeOnly time)
    {
        return new DinnerTime(null, PresentsType.Late, time);
    }
    
    public static DinnerTime CreateNotPresent()
    {
        return new DinnerTime(null, PresentsType.NotPresent);
    }

    private DinnerTime WithId(int? id)
    {
        Id = id;
        return this;
    }

    public DinnerTime Update(PresentsType presentsType, TimeOnly? time)
    {
        return presentsType switch
        {
            PresentsType.Unknown => !time.HasValue ? CreateUnknown().WithId(Id) : throw new InvalidModelException("Can't set Time for Type Unknown."),
            PresentsType.Default => time.HasValue ? CreateDefault(time.Value).WithId(Id) : throw new InvalidModelException("Must provide a Time for Default Type."),
            PresentsType.Late => time.HasValue ? CreateLate(time.Value).WithId(Id) : throw new InvalidModelException("Must provide a Time for Late Type."),
            PresentsType.NotPresent => !time.HasValue ? CreateNotPresent().WithId(Id) : throw new InvalidModelException("Can't set Time for Type NotPresent."),
            _ => throw new ArgumentOutOfRangeException(nameof(presentsType), presentsType, null)
        };
    }
}