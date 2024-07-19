using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Services.Persons;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController(IPersonService personService) : WhiIsHomeControllerBase<Person, PersonModel>
{
    [HttpGet("{id}")]
    public async Task<ActionResult<PersonModel>> GetPerson(string id, CancellationToken cancellationToken)
    {
        var result = await personService.GetAsync(id, cancellationToken);
        return BuildResponse(result, PersonModel.From);
    }
    
    [HttpGet]
    public async Task<ActionResult<PersonModel>> GetPersonByEmailAsync(string email, CancellationToken cancellationToken)
    {
         var result = await personService.GetByMailAsync(email, cancellationToken);
         return BuildResponse(result, PersonModel.From);
    }

    [HttpPost]
    public async Task<ActionResult<PersonModel>> CreatePersonAsync([FromBody] NewPersonModel person, CancellationToken cancellationToken)
    {
        var result = await personService.CreateAsync(person.DisplayName, person.Email, cancellationToken);
        return BuildResponse(result, PersonModel.From);
    }
}
