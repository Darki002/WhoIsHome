using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using WhoIsHome.WebApi.Models.New;
using WhoIsHome.WebApi.Models.Request;
using WhoIsHome.WebApi.Models.Response;

namespace WhoIsHome.WebApi.AggregatesControllers;

public class RepeatedEventController(IRepeatedEventAggregateService repeatedEventAggregateService, IUserContext userContext)
    : AggregateControllerBase<RepeatedEvent, RepeatedEventModelResponse>(repeatedEventAggregateService, userContext)
{
    [HttpPost]
    public async Task<ActionResult<RepeatedEventModelResponse>> CreateEvent([FromBody] NewRepeatedEventModel eventModel,
        CancellationToken cancellationToken)
    {
        var result = await repeatedEventAggregateService.CreateAsync(
            title: eventModel.Title,
            firstOccurrence: eventModel.FirstOccurrence,
            lastOccurrence: eventModel.LastOccurrence,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            presenceType: PresenceTypeHelper.FromString(eventModel.PresenceType),
            time: eventModel.DinnerTime,
            cancellationToken: cancellationToken);

        return await BuildResponseAsync(result);
    }

    [HttpPut]
    public async Task<ActionResult<RepeatedEventModelResponse>> UpdateEvent([FromBody] RepeatedEventModel eventModel,
        CancellationToken cancellationToken)
    {
        var result = await repeatedEventAggregateService.UpdateAsync(
            id: eventModel.Id,
            title: eventModel.Title,
            firstOccurrence: eventModel.FirstOccurrence,
            lastOccurrence: eventModel.LastOccurrence,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            presenceType: PresenceTypeHelper.FromString(eventModel.PresenceType),
            time: eventModel.DinnerTime,
            cancellationToken: cancellationToken);

        return await BuildResponseAsync(result);
    }

    protected override Task<RepeatedEventModelResponse> ConvertToModelAsync(RepeatedEvent data, User user) =>
        Task.FromResult(RepeatedEventModelResponse.From(data, user));
}