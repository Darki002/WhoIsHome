using WhoIsHome.Aggregates;
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
        (
            id: null,
            userName: userName,
            email: email,
            password: password
        );
    }
}