using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Services;

namespace WhoIsHome.WebApi.ModelControllers;

public abstract class ModelControllerBase<T, TModel>(IService<T> service) : WhoIsHomeControllerBase<T, TModel> where T : class
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TModel>> Get(string id, CancellationToken cancellationToken)
    {
        var result = await service.GetAsync(id, cancellationToken);
        return BuildResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TModel>> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsOk ? Ok() : BadRequest(result.Err.Unwrap());
    }
}