using Galaxus.Functional;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Services;

namespace WhoIsHome.WebApi.Controllers;

public abstract class WhoIsHomeControllerBase<T, TModel>(IService<T> service) : ControllerBase where T : class
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TModel>> GetEvent(string id, CancellationToken cancellationToken)
    {
        var result = await service.GetAsync(id, cancellationToken);
        return BuildResponse(result);
    }
    
    protected ActionResult<TModel> BuildResponse(Result<T, string> result)
    {
        if (result.IsErr)
        {
            return BadRequest(result.Err.Unwrap());
        }

        var model = ConvertToModel(result.Unwrap());
        return Ok(model);
    }

    protected abstract TModel ConvertToModel(T data);
}