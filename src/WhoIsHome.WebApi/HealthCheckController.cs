using Microsoft.AspNetCore.Mvc;

namespace WhoIsHome.WebApi;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok();
    }
}