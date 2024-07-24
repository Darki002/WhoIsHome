using WhoIsHome.WebApi.ModelControllers.Models;

namespace WhoIsHome.WebApi.DailyOverviews;

public record DailyOverviewModel
{
    public required PersonModel Person { get; set; }

    public required bool IsAtHome { get; set; }

    public required string? DinnerAt { get; set; }
}