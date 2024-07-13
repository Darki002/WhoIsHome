using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Persons;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController(IPersonService personService) : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<PersonModel> GetPerson(string id)
    {
        Console.WriteLine("Hi Get");
        return new PersonModel
        {
            Id = id,
            DisplayName = "Llyn",
            Email = "llyn@gmx.net"
        };
    }
    
    [HttpGet]
    public async Task<ActionResult<PersonModel>> GetPersonByEmailAsync(string email)
    {
         var result = await personService.GetPersonByMailAsync(email);
         
         if (result.IsErr)
         {
             BadRequest(result.Err.Unwrap());
         }

         var model = PersonModel.From(result.Unwrap());
         return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<PersonModel>> CreatePersonAsync(PersonModel person)
    {
        var result = await personService.TryCreateAsync(person.DisplayName, person.Email);

        if (result.IsErr)
        {
            BadRequest(result.Err.Unwrap());
        }
        
        var model = PersonModel.From(result.Unwrap());
        return Ok(model);
    }
}
