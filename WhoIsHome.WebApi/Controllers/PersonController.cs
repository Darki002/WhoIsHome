using Microsoft.AspNetCore.Mvc;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonController : ControllerBase
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

    [HttpPost]
    public ActionResult<PersonModel> CreatePerson(PersonModel person)
    {
        Console.WriteLine(person.DisplayName);
        return Ok(person);
    }
}
