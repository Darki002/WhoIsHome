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
            Name = "Llyn",
            Email = "llyn@gmx.net"
        };
    }

    [HttpPost]
    public ActionResult<PersonModel> CreatePerson(PersonModel person)
    {
        var result = personService.TryCreate(person.Name, person.Email);

        if (result.IsErr)
        {
            BadRequest(result.Err.Unwrap());
        }
        
        return Ok(result.Unwrap());
    }
}
