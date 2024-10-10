using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Aggregates.Mappers;

public static class OneTimeEventMapper
{
	public static OneTimeEvent ToAggregate(this OneTimeEventModel model)
	{
		var dinnerTime = new DinnerTime(model.PresenceType, model.DinnerTime);
		
		return new OneTimeEvent(
		model.Id,
		model.Title,
		model.Date,
		model.StartTime,
		model.EndTime,
		dinnerTime,
		model.UserModel.Id);
	}

	public static OneTimeEventModel ToModel(this OneTimeEvent aggregate, UserModel userModel)
	{
		var model = new OneTimeEventModel
		{
			Id = aggregate.Id!.Value,
			Date = aggregate.Date,
			Title = aggregate.Title,
			StartTime = aggregate.StartTime,
			EndTime = aggregate.EndTime,
			PresenceType = aggregate.DinnerTime.PresenceType,
			DinnerTime = aggregate.DinnerTime.Time,
			UserModel = userModel
		};

		if (aggregate.Id.HasValue)
		{
			model.Id = aggregate.Id.Value;
		}

		return model;
	}
}
