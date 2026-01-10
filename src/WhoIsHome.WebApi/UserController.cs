using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Entities;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi;

[Authorize]
[Route("api/v1/user")]
public class UserController(IUserContextProvider contextProvider, IUserService service) : Controller
{
    [HttpGet]
    public async Task<ActionResult<UserModel>> GetMe(CancellationToken cancellationToken)
    {
        return await GetUser(contextProvider.UserId, cancellationToken);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserModel>> GetUser(int id, CancellationToken cancellationToken)
    {
        var user = await service.GetAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound($"No User with id {id} found.");
        }
        
        return Ok(ToModel(user));
    }
    
    private static UserModel ToModel(User data)
    {
        return new UserModel
        {
            Id = data.Id,
            UserName = data.UserName,
            Email = data.Email
        };
    }
}