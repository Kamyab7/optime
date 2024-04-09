using Application.Common.Models;
using Application.Missions.Commands.AssignMission;
using Application.Missions.Commands.CreateMission;
using Application.Missions.Queries.GetMissionsWithPagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.ActionFilters;

namespace WebAPI.Controllers;

public class MissionController : ControllerBase
{
    private readonly ISender _sender;

    public MissionController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("/missions/create")]
    [AdminKeyRequired]
    public async Task<string> CreateDriver(CreateMissionCommand command)
    {
        return await _sender.Send(command);
    }

    [HttpPost("/missions/assign")]
    [AdminKeyRequired]
    public async Task<Unit> AssignMissionToDriver(AssignMissionCommand command)
    {
        return await _sender.Send(command);
    }

    [HttpGet("/missions")]
    [AdminKeyRequired]
    public async Task<PaginatedList<MissionDto>> GetMissionsWithPagination([AsParameters] GetMissionsWithPaginationQuery query)
    {
        return await _sender.Send(query);
    }
}
