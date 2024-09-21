using WhoIsHome.Aggregates;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverview
{
    public required User User { get; init; }

    public bool IsAtHome { get; init; } = true;

    public TimeOnly? DinnerAt { get; init; }

    public static DailyOverview Empty(User user)
    {
        return new DailyOverview
        {
            User = user
        };
    }

    public static DailyOverview NotAtHome(User user)
    {
        return new DailyOverview
        {
            User = user,
            IsAtHome = false,
            DinnerAt = null
        };
    }

    public static DailyOverview From(User user, DinnerTime dinnerTime)
    {
        return new DailyOverview
        {
            User = user,
            IsAtHome = dinnerTime.IsAtHome,
            DinnerAt = dinnerTime.Time
        };
    }
    
    private static TimeOnly? TryGetTimeOnly(int? timestamp) => timestamp?.ToTimeOnly();
}