using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Entities;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi;

public class UserController(IUserContext context, IUserService service) 
    : WhoIsHomeControllerBase<User, UserModel>
{
    [HttpGet("Me")]
    public async Task<ActionResult<UserModel>> GetMe(CancellationToken cancellationToken)
    {
        return await GetUser(context.UserId, cancellationToken);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserModel>> GetUser(int id, CancellationToken cancellationToken)
    {
        var user = await service.GetAsync(id, cancellationToken);
        return await BuildResponseAsync(user);
    }
    
    protected override Task<UserModel> ConvertToModelAsync(User data)
    {
        return Task.FromResult(new UserModel
        {
            Id = data.Id!.Value,
            UserName = data.UserName,
            Email = data.Email
        });
    }
}