using WhoIsHome.External.Models;

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
		model.UserId);
	}

	public static OneTimeEventModel ToModel(this OneTimeEvent aggregate)
	{
		var model = new OneTimeEventModel
		{
			Date = aggregate.Date,
			Title = aggregate.Title,
			StartTime = aggregate.StartTime,
			EndTime = aggregate.EndTime,
			PresenceType = aggregate.DinnerTime.PresenceType,
			DinnerTime = aggregate.DinnerTime.Time,
			UserId = aggregate.UserId
		};

		if (aggregate.Id.HasValue)
		{
			model.Id = aggregate.Id.Value;
		}

		return model;
	}
}
