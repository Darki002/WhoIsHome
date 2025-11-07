using WhoIsHome.Entities;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverview
{
    public User User { get; init; } = null!;

    public bool IsAtHome { get; private init; } = true;

    public TimeOnly? DinnerTime { get; private init; } = null;
    
    public string? ErrorMessage { get; set; }

    public bool HasError => ErrorMessage is not null;

    public static DailyOverview Empty(User user)
    {
        return new DailyOverview
        {
            User = user
        };
    }

    public static DailyOverview From(EventInstance eventInstance)
    {
        return new DailyOverview
        {
            User = eventInstance.User,
            IsAtHome = eventInstance.IsAtHome,
            DinnerTime = eventInstance.DinnerTime
        };
    }
    
    public static DailyOverview Error(string message)
    {
        return new DailyOverview
        {
            ErrorMessage = message
        };
    }
}