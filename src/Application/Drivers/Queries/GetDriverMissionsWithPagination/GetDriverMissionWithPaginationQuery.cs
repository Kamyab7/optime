using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mapping;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Application.Drivers.Queries.GetDriverMissionsWithPagination;

public record GetDriverMissionWithPaginationQuery : IRequest<PaginatedList<DriverMissionsDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class DriverMissionsDto
{
    public string Id { get; set; }

    public PointDto Destination { get; set; }

    public PointDto Source { get; set; }

    public MissionStatus MissionStatus { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Mission, DriverMissionsDto>()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                   .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => MapToPointDto(src.Destination)))
                   .ForMember(dest => dest.Source, opt => opt.MapFrom(src => MapToPointDto(src.Source)))
                   .ForMember(dest => dest.MissionStatus, opt => opt.MapFrom(src => src.MissionStatus));
        }
    }

    private static PointDto MapToPointDto(Point point)
    {
        return new PointDto
        {
            Latitude = point.Y,
            Longitude = point.X
        };
    }
}

public class GetDriverMissionWithPaginationQueryHandler : IRequestHandler<GetDriverMissionWithPaginationQuery, PaginatedList<DriverMissionsDto>>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    private readonly ICurrentUser _currentUser;

    public GetDriverMissionWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;

        _mapper = mapper;

        _currentUser = currentUser;
    }

    public async Task<PaginatedList<DriverMissionsDto>> Handle(GetDriverMissionWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var driverId = _currentUser.DriverId;

        if (driverId == null)
        {
            throw new ForbiddenAccessException();
        }

        return await _context.Missions.AsNoTracking()
            .Where(m => m.DriverId == driverId)
            .ProjectTo<DriverMissionsDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}