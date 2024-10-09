using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Types;
using WhoIsHome.WebApi.Models.New;
using WhoIsHome.WebApi.Models.Response;
using OneTimeEventModel = WhoIsHome.WebApi.Models.Request.OneTimeEventModel;

namespace WhoIsHome.WebApi.AggregatesControllers;

public class OneTimeEventController(
    OneTimeEventAggregateAggregateService oneTimeEventAggregateAggregateService, IUserService userService)
    : AggregateControllerBase<OneTimeEvent, OneTimeEventModelResponse>(oneTimeEventAggregateAggregateService, userService)
{
    [HttpPost]
    public async Task<ActionResult<OneTimeEventModelResponse>> CreateEvent([FromBody] NewOneTimeEventModel eventModel,
        CancellationToken cancellationToken)
    {
        var result = await oneTimeEventAggregateAggregateService.CreateAsync(
            title: eventModel.Title,
            date: eventModel.Date,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            presenceType: PresenceTypeHelper.FromString(eventModel.PresenceType),
            time: eventModel.DinnerTime,
            cancellationToken: cancellationToken);

        return await BuildResponseAsync(result);
    }

    [HttpPut]
    public async Task<ActionResult<OneTimeEventModelResponse>> UpdateEvent([FromBody] OneTimeEventModel eventModel,
        CancellationToken cancellationToken)
    {
        var result = await oneTimeEventAggregateAggregateService.UpdateAsync(
            id: eventModel.Id,
            title: eventModel.Title,
            date: eventModel.Date,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            presenceType: PresenceTypeHelper.FromString(eventModel.PresenceType),
            time: eventModel.DinnerTime,
            cancellationToken: cancellationToken);

        return await BuildResponseAsync(result);
    }

    protected override Task<OneTimeEventModelResponse> ConvertToModelAsync(OneTimeEvent data, User user) =>
        Task.FromResult(OneTimeEventModelResponse.From(data, user));
}