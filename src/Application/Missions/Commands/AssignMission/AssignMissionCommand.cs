using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Missions.Commands.AssignMission;

public record AssignMissionCommand : IRequest<Unit>
{
    public string MissionId { get; init; } = null!;

    public string DriverId { get; init; } = null!;
}

public class AssignMissionCommandValidator : AbstractValidator<AssignMissionCommand>
{
    public AssignMissionCommandValidator()
    {
        RuleFor(a => a.DriverId)
            .NotNull()
            .NotEmpty();

        RuleFor(a => a.MissionId)
            .NotNull()
            .NotEmpty();
    }
}

public class AssignMissionCommandHandler : IRequestHandler<AssignMissionCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public AssignMissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AssignMissionCommand request, CancellationToken cancellationToken)
    {
        var driver = await _context.Drivers
            .FindAsync(new object[] { request.DriverId }, cancellationToken);

        if (driver == null)
            throw new NotFoundException();

        var hasInProgressMission = await _context.Missions.AnyAsync(x => x.MissionStatus == Domain.MissionStatus.InProgress && x.DriverId == driver.Id);

        if (hasInProgressMission)
            throw new ArgumentException("driver already has a InProgress mission.");

        var mission = await _context.Missions
            .FindAsync(new object[] { request.MissionId }, cancellationToken);

        if (mission == null)
            throw new NotFoundException();

        switch (mission.MissionStatus)
        {
            case MissionStatus.InProgress:
                throw new ArgumentException("mission is already assigned.");
            case MissionStatus.Done:
                throw new ArgumentException("mission status is done.");
        }

        mission.AssignToDriver(driver);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}