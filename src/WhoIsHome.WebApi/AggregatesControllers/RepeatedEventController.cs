using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Helper;
using WhoIsHome.WebApi.Models.Dto;
using WhoIsHome.WebApi.Models.Response;

namespace WhoIsHome.WebApi.AggregatesControllers;

public class RepeatedEventController(IRepeatedEventAggregateService repeatedEventAggregateService, IUserContext userContext)
    : AggregateControllerBase<RepeatedEvent, RepeatedEventModel>(repeatedEventAggregateService, userContext)
{
    [HttpPost]
    public async Task<ActionResult<RepeatedEventModel>> CreateEvent([FromBody] RepeatedEventModelDto eventModelDto,
        CancellationToken cancellationToken)
    {
        var result = await repeatedEventAggregateService.CreateAsync(
            title: eventModelDto.Title,
            firstOccurrence: eventModelDto.FirstOccurrence,
            lastOccurrence: eventModelDto.LastOccurrence,
            startTime: eventModelDto.StartTime,
            endTime: eventModelDto.EndTime,
            presenceType: PresenceTypeHelper.FromString(eventModelDto.PresenceType),
            time: eventModelDto.DinnerTime,
            cancellationToken: cancellationToken);

        return await BuildResponseAsync(result);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<RepeatedEventModel>> UpdateEvent(
        int id,
        [FromBody] RepeatedEventModelDto eventModel,
        CancellationToken cancellationToken)
    {
        var result = await repeatedEventAggregateService.UpdateAsync(
            id: id,
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

    protected override Task<RepeatedEventModel> ConvertToModelAsync(RepeatedEvent data, User user) =>
        Task.FromResult(RepeatedEventModel.From(data, user));
}