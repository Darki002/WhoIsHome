using WhoIsHome.External.Models;

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
			var model = new UserModel
			{
				UserName = aggregate.UserName,
				Email = aggregate.Email,
				Password = aggregate.Password
			};

			if (aggregate.Id.HasValue)
			{
				model.Id = aggregate.Id.Value;
			}

			return model;
		}
	}
}
