using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.AggregatesControllers;

public abstract class AggregateControllerBase<T, TModel>(IAggregateService<T> aggregateService, IUserService userService) : WhoIsHomeControllerBase<T, TModel>
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TModel>> Get(int id, CancellationToken cancellationToken)
    {
        var result = await aggregateService.GetAsync(id, cancellationToken);
        return await BuildResponseAsync(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TModel>> Delete(int id, CancellationToken cancellationToken)
    {
        await aggregateService.DeleteAsync(id, cancellationToken);
        return Ok();
    }

    protected override async Task<TModel> ConvertToModelAsync(T data)
    {
        var user = await userService.GetCurrentUserAsync();
        return await ConvertToModelAsync(data, user.ToUser());
    }

    protected abstract Task<TModel> ConvertToModelAsync(T data, User user);
}