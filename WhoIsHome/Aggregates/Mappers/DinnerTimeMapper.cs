using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Aggregates.Mappers;

public static class DinnerTimeMapper
{
	public static DinnerTime ToAggregate(this DinnerTimeModel model)
	{
		return new DinnerTime(
		model.Id,
		model.PresentsType,
		model.Time);
	}

	public static DinnerTimeModel ToModel(this DinnerTime aggregate)
	{
		return new DinnerTimeModel
		{
			Id = aggregate.Id!.Value,
			PresentsType = aggregate.PresentsType,
			Time = aggregate.Time
		};
	}
}
