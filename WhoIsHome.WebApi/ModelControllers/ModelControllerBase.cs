using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Services;

namespace WhoIsHome.WebApi.ModelControllers;

public abstract class ModelControllerBase<T, TModel>(IService<T> service) : WhoIsHomeControllerBase<T, TModel> where T : class
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TModel>> GetEvent(string id, CancellationToken cancellationToken)
    {
        var result = await service.GetAsync(id, cancellationToken);
        return BuildResponse(result);
    }
}