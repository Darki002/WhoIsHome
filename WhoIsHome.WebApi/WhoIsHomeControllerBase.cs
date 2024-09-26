using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WhoIsHome.WebApi;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public abstract class WhoIsHomeControllerBase<T, TModel> : ControllerBase
{
    protected async Task<ActionResult<TModel>> BuildResponseAsync(T result)
    {
        var model = await ConvertToModelAsync(result);
        return Ok(model);
    }
    
    protected abstract Task<TModel> ConvertToModelAsync(T data);
}