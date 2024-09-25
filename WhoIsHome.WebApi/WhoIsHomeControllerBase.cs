using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhoIsHome.WebApi;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public abstract class WhoIsHomeControllerBase<T, TModel> : ControllerBase
{
    protected ActionResult<TModel> BuildResponse(T result)
    {
        var model = ConvertToModel(result);
        return Ok(model);
    }
    
    protected abstract TModel ConvertToModel(T data);
}