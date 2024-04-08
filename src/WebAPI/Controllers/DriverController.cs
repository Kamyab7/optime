using Application.Common.Models;
using Application.Drivers.Commands.CreateDriver;
using Application.Drivers.Queries.GetDriverWithPagination;
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

    [HttpGet("/drivers")]
    public async Task<PaginatedList<DriverDto>> GetDriversWithPagination([AsParameters] GetDriverWithPaginationQuery query)
    {
        return await _sender.Send(query);
    }
}
