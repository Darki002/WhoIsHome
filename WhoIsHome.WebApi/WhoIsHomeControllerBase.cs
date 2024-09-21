using Microsoft.AspNetCore.Mvc;

namespace WhoIsHome.WebApi;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class WhoIsHomeControllerBase<T, TModel> : ControllerBase
{
    protected ActionResult<TModel> BuildResponse(T result)
    {
        var model = ConvertToModel(result);
        return Ok(model);
    }
    
    protected ActionResult<IReadOnlyList<TModel>> BuildResponse(IReadOnlyList<T> result)
    {
        var model = result.Select(ConvertToModel).ToList();
        return Ok(model);
    }
    
    protected abstract TModel ConvertToModel(T data);
}