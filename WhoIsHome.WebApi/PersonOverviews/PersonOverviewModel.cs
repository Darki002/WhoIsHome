using WhoIsHome.WebApi.ModelControllers.Models;

namespace WhoIsHome.WebApi.PersonOverviews;

public record PersonOverviewModel
{
    public required PersonModel Person { get; init; }
    
    public required IReadOnlyList<PersonOverviewEventModel> Events { get; init; }
}