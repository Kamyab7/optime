using Application.Common.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Missions.Commands.CreateMission;

public record CreateMissionCommand : IRequest<string>
{
    public double DestinationLatitude { get; init; }
    public double DestinationLongitude { get; init; }

    public double SourceLatitude { get; init; }
    public double SourceLongitude { get; init; }
}

public class CreateMissionCommandValidator : AbstractValidator<CreateMissionCommand>
{
    public CreateMissionCommandValidator()
    {
        RuleFor(command => command.DestinationLatitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(command => command.DestinationLongitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees.");

        RuleFor(command => command.SourceLatitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(command => command.SourceLongitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees.");
    }
}

public class CreateMissionCommandHandler : IRequestHandler<CreateMissionCommand, string>
{
    private readonly IApplicationDbContext _context;

    public CreateMissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
    {
        // TODO: Assign mission here

        var destinationPoint = new Point(request.DestinationLongitude, request.DestinationLatitude) { SRID = 4326 }; // Assuming WGS 84 coordinate system

        var sourcePoint = new Point(request.SourceLongitude, request.SourceLatitude) { SRID = 4326 }; // Assuming WGS 84 coordinate system

        var mission = new Mission(destinationPoint, sourcePoint);

        _context.Missions.Add(mission);

        await _context.SaveChangesAsync(cancellationToken);

        return mission.Id;
    }
}
