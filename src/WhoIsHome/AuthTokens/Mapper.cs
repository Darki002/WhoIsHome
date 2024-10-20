using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.AuthTokens;

public static class Mapper
{
    public static RefreshTokenModel ToModel(this RefreshToken refreshToken)
    {
        var model = new RefreshTokenModel
        {
            Token = refreshToken.Token,
            Issued = refreshToken.Issued
        };
            
        if (refreshToken.Id.HasValue)
        {
            model.Id = refreshToken.Id.Value;
        }
        return model;
    }

    public static RefreshToken ToRefreshToken(this RefreshTokenModel model)
    {
        return new RefreshToken(model.Id, model.Token, model.Issued);
    }
}