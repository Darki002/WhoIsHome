﻿using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.WebApi.Models.New;

public class NewRepeatedEventModel
{
    public required string Title { get; set; }
    
    public required DateOnly FirstOccurrence { get; set; }
    
    public required DateOnly LastOccurrence { get; set; }

    public required TimeOnly StartTime { get; set; }

    public required TimeOnly EndTime { get; set; }

    public required string PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; } = null;
}