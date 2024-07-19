using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Persons;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController(IPersonService personService) : WhiIsHomeControllerBase<Person, PersonModel>
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPerson(string id, CancellationToken cancellationToken)
    {
        var result = await personService.GetAsync(id, cancellationToken);
        return BuildResponse(result, PersonModel.From);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPersonByEmailAsync(string email, CancellationToken cancellationToken)
    {
         var result = await personService.GetByMailAsync(email, cancellationToken);
         return BuildResponse(result, PersonModel.From);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePersonAsync([FromBody] NewPersonModel person, CancellationToken cancellationToken)
    {
        var result = await personService.CreateAsync(person.DisplayName, person.Email, cancellationToken);
        return BuildResponse(result, PersonModel.From);
    }
}
