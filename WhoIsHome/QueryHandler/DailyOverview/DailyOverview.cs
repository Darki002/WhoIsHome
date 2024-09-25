using WhoIsHome.Aggregates;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverview
{
    public required User User { get; init; }

    public bool IsAtHome { get; init; } = true;

    public TimeOnly? DinnerTime { get; init; }

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
            DinnerTime = null
        };
    }

    public static DailyOverview From(User user, DinnerTime dinnerTime)
    {
        return new DailyOverview
        {
            User = user,
            IsAtHome = dinnerTime.IsAtHome,
            DinnerTime = dinnerTime.Time
        };
    }
}