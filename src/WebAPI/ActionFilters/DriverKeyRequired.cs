﻿using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace WebAPI.ActionFilters;

public class DriverKeyRequired : ActionFilterAttribute
{
    private const string ApiKeyName = "X-API-KEY";

    private readonly IApplicationDbContext _context;

    public DriverKeyRequired(IApplicationDbContext context)
    {
        _context = context;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyName, out StringValues extractedApiKeys))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        // Convert StringValues to string
        string extractedApiKey = extractedApiKeys.FirstOrDefault();
        var driver = _context.Drivers.FirstOrDefault(x => x.ApiKey == extractedApiKey);

        if (driver == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["DriverId"] = driver.Id;

        base.OnActionExecuting(context);
    }
}
