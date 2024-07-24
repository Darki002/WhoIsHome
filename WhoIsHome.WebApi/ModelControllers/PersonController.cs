using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Services.Persons;
using WhoIsHome.WebApi.ModelControllers.Models;

namespace WhoIsHome.WebApi.ModelControllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController(IPersonService personService) : ModelControllerBase<Person, PersonModel>(personService)
{
    [HttpGet]
    public async Task<ActionResult<PersonModel>> GetPersonByEmailAsync(string email, CancellationToken cancellationToken)
    {
         var result = await personService.GetByMailAsync(email, cancellationToken);
         return BuildResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult<PersonModel>> CreatePersonAsync([FromBody] NewPersonModel person, CancellationToken cancellationToken)
    {
        var result = await personService.CreateAsync(person.DisplayName, person.Email, cancellationToken);
        return BuildResponse(result);
    }

    protected override PersonModel ConvertToModel(Person data) => PersonModel.From(data);
}
