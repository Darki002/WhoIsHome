using WhoIsHome.Aggregates;

namespace WhoIsHome.WebApi.Models;

public class UserSettingsDto
{
    public TimeOnly? DefaultDinnerTime { get; private set; }

    public UserSettingsDto From(UserSettings userSettings)
    {
        return new UserSettingsDto
        {
            DefaultDinnerTime = userSettings.DefaultDinnerTime
        };
    }
}