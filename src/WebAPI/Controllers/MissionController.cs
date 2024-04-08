using Application.Missions.Commands.CreateMission;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class MissionController : ControllerBase
{
    private readonly ISender _sender;

    public MissionController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("/missions/create")]
    public async Task<string> CreateDriver(CreateMissionCommand command)
    {
        return await _sender.Send(command);
    }
}
