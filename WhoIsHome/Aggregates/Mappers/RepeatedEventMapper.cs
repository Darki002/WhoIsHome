using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Aggregates.Mappers;

public static class RepeatedEventMapper
{
	public static RepeatedEvent ToAggregate(this RepeatedEventModel model)
	{
		var dinnerTime = new DinnerTime(model.PresenceType, model.DinnerTime);

		return new RepeatedEvent(
		model.Id,
		model.Title,
		model.FirstOccurrence,
		model.LastOccurrence,
		model.StartTime,
		model.EndTime,
		dinnerTime,
		model.UserModel.Id);
	}

	public static RepeatedEventModel ToModel(this RepeatedEvent aggregate, UserModel userModel)
	{
		return new RepeatedEventModel
		{
			Id = aggregate.Id!.Value,
			FirstOccurrence = aggregate.FirstOccurrence,
			LastOccurrence = aggregate.LastOccurrence,
			Title = aggregate.Title,
			StartTime = aggregate.StartTime,
			EndTime = aggregate.EndTime,
			PresenceType = aggregate.DinnerTime.PresenceType,
			DinnerTime = aggregate.DinnerTime.Time,
			UserModel = userModel
		};
	}
}
