using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Persons;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController(IPersonService personService) : WhiIsHomeControllerBase<Person, PersonModel>
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPerson(string id)
    {
        var result = await personService.GetAsync(id);
        return BuildResponse(result, PersonModel.From);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPersonByEmailAsync(string email)
    {
         var result = await personService.GetByMailAsync(email);
         return BuildResponse(result, PersonModel.From);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePersonAsync(PersonModel person)
    {
        var result = await personService.CreateAsync(person.DisplayName, person.Email);
        return BuildResponse(result, PersonModel.From);
    }
}
