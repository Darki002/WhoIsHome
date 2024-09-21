using WhoIsHome.Shared;

namespace WhoIsHome.Models;

public class DinnerTime
{
    public int? Id { get; set; }

    public PresentsType PresentsType { get; set; }
    
    public TimeOnly? Time { get; set; }

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
}