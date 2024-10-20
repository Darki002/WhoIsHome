using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.AuthTokens;

public static class Mapper
{
    public static RefreshTokenModel ToModel(this RefreshToken refreshToken, User user)
    {
        var model = new RefreshTokenModel
        {
            Token = refreshToken.Token,
            Issued = refreshToken.Issued,
            UserModel = user.ToModel()
        };
            
        if (refreshToken.Id.HasValue)
        {
            model.Id = refreshToken.Id.Value;
        }
        return model;
    }

    public static RefreshToken ToRefreshToken(this RefreshTokenModel model)
    {
        return new RefreshToken(model.Id,model.UserModel.Id, model.Token, model.Issued);
    }
}