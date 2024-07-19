using WhoIsHome.Services.Persons;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class PersonPresence
{
    public Person Person { get; set; }
    
    public bool IsAtHome { get; set; }
    
    public TimeOnly DinnerAt { get; set; }
}