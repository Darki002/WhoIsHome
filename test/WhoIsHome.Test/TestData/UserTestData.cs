using WhoIsHome.Entities;

namespace WhoIsHome.Test.TestData;

public static class UserTestData
{
    public static User CreateDefaultUser(
        int id = 1,
        string userName = "Test User", 
        string email = "test.user@whoishome.dev", 
        string password = "test")
    {
        return new User
        {
            Id = id,
            UserName = userName,
            Email = email,
            Password =  password
        };
    }
}