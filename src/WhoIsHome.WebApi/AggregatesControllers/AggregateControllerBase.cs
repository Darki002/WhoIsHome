using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.AggregatesControllers;

public abstract class AggregateControllerBase<T, TModel>(IAggregateService<T> aggregateService, IUserContext userContext) 
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

    protected override async Task<TModel> ConvertToModelAsync(T data)
    {
        var user = await userContext.GetCurrentUserAsync();
        return await ConvertToModelAsync(data, user.ToUser());
    }

    protected abstract Task<TModel> ConvertToModelAsync(T data, User user);
}