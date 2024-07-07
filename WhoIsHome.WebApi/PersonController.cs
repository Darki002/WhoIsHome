using Microsoft.AspNetCore.Mvc;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public string GetPerson()
        {
            return "Hi";
        }
    }
}
