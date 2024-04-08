using Application.Common.Interfaces;
using Application.Common.Mapping;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Application.Missions.Queries;

public record GetMissionsWithPaginationQuery : IRequest<PaginatedList<MissionDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class MissionDto
{
    public string Id { get; set; }

    public PointDto Destination { get; set; }

    public PointDto Source { get; set; }

    public MissionStatus MissionStatus { get; set; }

    public string? DriverId { get; set; }

    public Driver Driver { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Mission, MissionDto>()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                   .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => MapToPointDto(src.Destination)))
                   .ForMember(dest => dest.Source, opt => opt.MapFrom(src => MapToPointDto(src.Source)))
                   .ForMember(dest => dest.MissionStatus, opt => opt.MapFrom(src => src.MissionStatus))
                   .ForMember(dest => dest.DriverId, opt => opt.MapFrom(src => src.DriverId))
                   .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver));
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

    public class PointDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

public class GetMissionsWithPaginationQueryHandler : IRequestHandler<GetMissionsWithPaginationQuery, PaginatedList<MissionDto>>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetMissionsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;

        _mapper = mapper;
    }

    public async Task<PaginatedList<MissionDto>> Handle(GetMissionsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Missions
            .AsNoTracking()
            .Include(m => m.Driver)
            .ProjectTo<MissionDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
