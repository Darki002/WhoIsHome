using WhoIsHome.Entities;
using WhoIsHome.External.Models;

namespace WhoIsHome.Test.Shared.Helper;

public static class Mappers
{
    public static UserModel ToModel(this User model)
    {
        return new UserModel
        {
            Id = model.Id!.Value,
            UserName = model.UserName,
            Email = model.Email,
            Password = model.Password
        };
    }
}