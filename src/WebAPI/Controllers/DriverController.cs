using Application.Drivers.Commands.CreateDriver;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class DriverController : ControllerBase
{
    private readonly ISender _sender;

    public DriverController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("/drivers/create")]
    public async Task<string> CreateDriver(CreateDriverCommand command)
    {
        return await _sender.Send(command);
    }
}
