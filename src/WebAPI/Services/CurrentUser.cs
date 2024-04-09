using Application.Common.Interfaces;

namespace WebAPI.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? DriverId => _httpContextAccessor.HttpContext?.Items["DriverId"]?.ToString();
}
