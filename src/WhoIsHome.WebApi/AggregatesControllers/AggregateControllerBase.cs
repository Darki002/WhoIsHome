using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.AggregatesControllers;

public abstract class AggregateControllerBase<T, TModel>(IAggregateService<T> aggregateService) 
    : WhoIsHomeControllerBase<T, TModel>
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TModel>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await aggregateService.GetAsync(id, cancellationToken);
        return await BuildResponseAsync(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<TModel>> Delete(int id, CancellationToken cancellationToken)
    {
        await aggregateService.DeleteAsync(id, cancellationToken);
        return Ok();
    }
}