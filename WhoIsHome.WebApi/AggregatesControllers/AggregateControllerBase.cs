using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;

namespace WhoIsHome.WebApi.AggregatesControllers;

public abstract class AggregateControllerBase<T, TModel>(IAggregateService<T> aggregateAggregateAggregateService) : WhoIsHomeControllerBase<T, TModel>
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TModel>> Get(int id, CancellationToken cancellationToken)
    {
        // TODO Authentication
        var result = await aggregateAggregateAggregateService.GetAsync(id, cancellationToken);
        return BuildResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TModel>> Delete(int id, CancellationToken cancellationToken)
    {
        // TODO Authentication
        await aggregateAggregateAggregateService.DeleteAsync(id, cancellationToken);
        return Ok();
    }

    protected ActionResult<TModel> BuildResponse(T result, User user)
    {
        var model = ConvertToModel(result, user);
        return Ok(model);
    }
    
    protected ActionResult<IReadOnlyList<TModel>> BuildResponse(IReadOnlyList<T> result, User user)
    {
        var model = result.Select(r => ConvertToModel(r, user)).ToList();
        return Ok(model);
    }

    protected override TModel ConvertToModel(T data)
    {
        return default!;
    }

    protected abstract TModel ConvertToModel(T data, User user);
}