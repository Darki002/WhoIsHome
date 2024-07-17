using Galaxus.Functional;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Persons;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController(IPersonService personService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPerson(string id)
    {
        var result = await personService.GetPersonAsync(id);
        return BuildResponse(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPersonByEmailAsync(string email)
    {
         var result = await personService.GetPersonByMailAsync(email);
         return BuildResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePersonAsync(PersonModel person)
    {
        var result = await personService.TryCreateAsync(person.DisplayName, person.Email);
        return BuildResponse(result);
    }

    private IActionResult BuildResponse(Result<Person, string> result)
    {
        if (result.IsErr)
        {
            return BadRequest(result.Err.Unwrap());
        }
        
        var model = PersonModel.From(result.Unwrap());
        return Ok(model);
    }
}
