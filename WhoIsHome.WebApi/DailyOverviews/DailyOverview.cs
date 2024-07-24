using WhoIsHome.WebApi.ModelControllers.Models;

namespace WhoIsHome.WebApi.DailyOverviews;

public record DailyOverview
{
    public required PersonModel Person { get; set; }

    public required bool IsAtHome { get; set; }

    public required string? DinnerAt { get; set; }
}