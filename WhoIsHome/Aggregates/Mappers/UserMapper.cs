using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Aggregates.Mappers
{
	public static class UserMapper
	{
		public static User ToAggregate(this UserModel model)
		{
			return new User(
				model.Id,
				model.UserName,
				model.Email,
				model.Password);
		}

		public static UserModel ToModel(this User aggregate)
		{
			return new UserModel
			{
				Id = aggregate.Id!.Value,
				UserName = aggregate.UserName,
				Email = aggregate.Email,
				Password = aggregate.Password
			};
		}
	}
}
