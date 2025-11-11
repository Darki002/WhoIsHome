using WhoIsHome.Entities;

namespace WhoIsHome.Test.TestData;

public static class UserTestData
{
    public static User CreateDefaultUser(
        string userName = "Test User", 
        string email = "test.user@whoishome.dev", 
        string password = "test")
    {
        return new User
        {
            Id = 0,
            UserName = userName,
            Email = email,
            Password =  password
        };
    }
}