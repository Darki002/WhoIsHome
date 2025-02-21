using WhoIsHome.Shared.BaseTypes;

namespace WhoIsHome.Aggregates;

public class UserSettings : AggregateBase
{
    public int? Id { get; }
    
    public int UserId { get; }
    
    public TimeOnly? DefaultDinnerTime { get; private set; }
    
    private UserSettings(int? id, int userId, TimeOnly? defaultDinnerTime)
    {
        Id = id;
        UserId = userId;
        DefaultDinnerTime = defaultDinnerTime;
    }

    public static UserSettings Create(int userId)
    {
        return new UserSettings(null, userId, null);
    }
}