using NetTopologySuite.Geometries;

namespace Domain;

public class Mission
{
    public string Id { get; set; }

    public Point Distination { get; set; }

    public Point Source { get; set; }

    public MissionStatus MissionStatus { get; set; }

    public string? DriverId { get; set; }

    public Driver Driver { get; set; }
}
