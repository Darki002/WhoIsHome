using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Helper;
using WhoIsHome.WebApi.Models.Dto;
using WhoIsHome.WebApi.Models.Response;

namespace WhoIsHome.WebApi.AggregatesControllers;

public class OneTimeEventController(
    IOneTimeEventAggregateService oneTimeEventAggregateService, IUserContext userContext)
    : AggregateControllerBase<OneTimeEvent, OneTimeEventModel>(oneTimeEventAggregateService, userContext)
{
    [HttpPost]
    public async Task<ActionResult<OneTimeEventModel>> CreateEvent(
        [FromBody] OneTimeEventModelDto eventModelDto,
        CancellationToken cancellationToken)
    {
        var result = await oneTimeEventAggregateService.CreateAsync(
            title: eventModelDto.Title,
            date: eventModelDto.Date,
            startTime: eventModelDto.StartTime,
            endTime: eventModelDto.EndTime,
            presenceType: PresenceTypeHelper.FromString(eventModelDto.PresenceType),
            time: eventModelDto.DinnerTime,
            cancellationToken: cancellationToken);

        return await BuildResponseAsync(result);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<OneTimeEventModel>> UpdateEvent(
        int id, 
        [FromBody] OneTimeEventModelDto eventModel,
        CancellationToken cancellationToken)
    {
        var result = await oneTimeEventAggregateService.UpdateAsync(
            id: id,
            title: eventModel.Title,
            date: eventModel.Date,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            presenceType: PresenceTypeHelper.FromString(eventModel.PresenceType),
            time: eventModel.DinnerTime,
            cancellationToken: cancellationToken);

        return await BuildResponseAsync(result);
    }

    protected override Task<OneTimeEventModel> ConvertToModelAsync(OneTimeEvent data, User user) =>
        Task.FromResult(OneTimeEventModel.From(data, user));
}