using Application.Common.Interfaces;
using Application.Common.Mapping;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Drivers.Queries.GetDriverWithPagination;

public record GetDriverWithPaginationQuery : IRequest<PaginatedList<DriverDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class DriverDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public bool HasInProgressMission { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Driver, DriverDto>()
                .ForMember(dest => dest.HasInProgressMission, opt => opt.MapFrom(src => src.Missions.Any(m => m.MissionStatus == MissionStatus.InProgress)));
        }
    }
}

public class GetDriverWithPaginationQueryHandler : IRequestHandler<GetDriverWithPaginationQuery, PaginatedList<DriverDto>>
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetDriverWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;

        _mapper = mapper;
    }

    public async Task<PaginatedList<DriverDto>> Handle(GetDriverWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Drivers
            .AsNoTracking()
            .Include(d => d.Missions)
            .ProjectTo<DriverDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}