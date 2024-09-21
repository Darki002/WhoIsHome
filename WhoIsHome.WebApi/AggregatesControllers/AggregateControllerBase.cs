using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Services;

namespace WhoIsHome.WebApi.AggregatesControllers;

public abstract class AggregateControllerBase<T, TModel>(IService<T> service) : WhoIsHomeControllerBase<T, TModel> where T : class
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TModel>> Get(int id, CancellationToken cancellationToken)
    {
        // TODO Authentication
        var result = await service.GetAsync(id, cancellationToken);
        return BuildResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TModel>> Delete(int id, CancellationToken cancellationToken)
    {
        // TODO Authentication
        await service.DeleteAsync(id, cancellationToken);
        return Ok();
    }
}