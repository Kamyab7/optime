using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Drivers.Commands.CreateDriver;
using Application.Drivers.Queries.GetDriverMissionsWithPagination;
using Application.Drivers.Queries.GetDriverWithPagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.ActionFilters;

namespace WebAPI.Controllers;

public class DriverController : ControllerBase
{
    private readonly ISender _sender;

    public DriverController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("/drivers/create")]
    [AdminKeyRequired]
    public async Task<string> CreateDriver(CreateDriverCommand command)
    {
        return await _sender.Send(command);
    }

    [HttpGet("/drivers")]
    [AdminKeyRequired]
    public async Task<PaginatedList<DriverDto>> GetDriversWithPagination([AsParameters] GetDriverWithPaginationQuery query)
    {
        return await _sender.Send(query);
    }

    [HttpGet("/drivers/missions")]
    [ServiceFilter(typeof(DriverKeyRequired))]
    public async Task<PaginatedList<DriverMissionsDto>> GetDriverMissionsWithPagination([AsParameters] GetDriverMissionWithPaginationQuery query)
    {
        return await _sender.Send(query);
    }
}
