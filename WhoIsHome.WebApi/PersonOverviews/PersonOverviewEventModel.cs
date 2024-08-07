﻿using WhoIsHome.QueryHandler.PersonOverview;

namespace WhoIsHome.WebApi.PersonOverviews;

public record PersonOverviewEventModel
{
    public required string Id { get; init; }
    
    public required string EventName { get; init; }
    
    public required DateTime Date { get; init; }
    
    public required DateTime StartTime { get; init; }
    
    public required DateTime EndTime { get; init; }
    
    public required EventType EventType { get; init; }

    public static PersonOverviewEventModel From(PersonOverviewEvent personOverviewEvent)
    {
        return new PersonOverviewEventModel
        {
            Id = personOverviewEvent.Id,
            EventName = personOverviewEvent.EventName,
            Date = personOverviewEvent.Date,
            StartTime = personOverviewEvent.StartTime,
            EndTime = personOverviewEvent.EndTime,
            EventType = personOverviewEvent.EventType
        };
    }
}