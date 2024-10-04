using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Aggregates.Mappers;

public static class DinnerTimeMapper
{
	public static DinnerTime ToAggregate(this DinnerTimeModel model)
	{
		return new DinnerTime(
		model.PresenceType,
		model.Time);
	}

	public static DinnerTimeModel ToModel(this DinnerTime aggregate)
	{
		return new DinnerTimeModel
		{
			Id = 1,
			PresenceType = aggregate.PresenceType,
			Time = aggregate.Time
		};
	}
}
