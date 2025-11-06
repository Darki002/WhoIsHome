using WhoIsHome.Entities;

namespace WhoIsHome.WebApi.Models;

public class UserModel
{
    public required int Id { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public static UserModel From(User user)
    {
        return new UserModel
        {
            Id = user.Id!.Value,
            Email = user.Email,
            UserName = user.UserName
        };
    }
}