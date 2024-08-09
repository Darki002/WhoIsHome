using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.PersonOverview;
using WhoIsHome.WebApi.ModelControllers.Models;

namespace WhoIsHome.WebApi.PersonOverviews;

public class PersonOverviewController(PersonOverviewQueryHandler queryHandler) : WhoIsHomeControllerBase<PersonOverview, PersonOverviewModel>
{
    [HttpGet("/{personId}")]
    public async Task<ActionResult<PersonOverviewModel>> GetPersonOverviewAsync(string personId, CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(personId, cancellationToken);
        return BuildResponse(result);
    }
    
    protected override PersonOverviewModel ConvertToModel(PersonOverview data)
    {
        return new PersonOverviewModel
        {
            Person = PersonModel.From(data.Person),
            Events = data.Events.Select(PersonOverviewEventModel.From).ToList()
        };
    }
}