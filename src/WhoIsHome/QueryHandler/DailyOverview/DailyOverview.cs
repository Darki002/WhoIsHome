using WhoIsHome.Aggregates;

namespace WhoIsHome.QueryHandler.DailyOverview;

// TODO: overthink this shit!!! Test it, seems to be wrong
public class DailyOverview
{
    public required User User { get; init; }

    public bool IsAtHome { get; private init; } = true;

    public TimeOnly? DinnerTime { get; private init; } = null!;

    public static DailyOverview Empty(User user)
    {
        return new DailyOverview
        {
            User = user
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