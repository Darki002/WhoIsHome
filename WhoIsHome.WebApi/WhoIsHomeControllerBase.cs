using Galaxus.Functional;
using Microsoft.AspNetCore.Mvc;

namespace WhoIsHome.WebApi;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class WhoIsHomeControllerBase<T, TModel> : ControllerBase where T : class
{
    protected ActionResult<TModel> BuildResponse(Result<T, string> result)
    {
        if (result.IsErr)
        {
            return BadRequest(result.Err.Unwrap());
        }

        var model = ConvertToModel(result.Unwrap());
        return Ok(model);
    }
    
    protected ActionResult<IReadOnlyList<TModel>> BuildResponse(Result<IReadOnlyList<T>, string> result)
    {
        if (result.IsErr)
        {
            return BadRequest(result.Err.Unwrap());
        }

        var model = result.Unwrap().Select(ConvertToModel).ToList();
        return Ok(model);
    }
    
    protected abstract TModel ConvertToModel(T data);
}