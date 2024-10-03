using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Aggregates.Mappers;

public static class OneTimeEventMapper
{
	public static OneTimeEvent ToAggregate(this OneTimeEventModel model)
	{
		return new OneTimeEvent(
		model.Id,
		model.Title,
		model.Date,
		model.StartTime,
		model.EndTime,
		model.DinnerTimeModel.ToAggregate(),
		model.UserModel.Id);
	}

	public static OneTimeEventModel ToModel(this OneTimeEvent aggregate, UserModel userModel)
	{
		return new OneTimeEventModel
		{
			Id = aggregate.Id!.Value,
			Date = aggregate.Date,
			Title = aggregate.Title,
			StartTime = aggregate.StartTime,
			EndTime = aggregate.EndTime,
			DinnerTimeModel = aggregate.DinnerTime.ToModel(),
			UserModel = userModel
		};
	}
}
