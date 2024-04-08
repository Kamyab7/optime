using NetTopologySuite.Geometries;

namespace Domain;

public class Mission
{
    public string Id { get; set; }

    public Point Destination { get; set; }

    public Point Source { get; set; }

    public MissionStatus MissionStatus { get; set; }

    public string? DriverId { get; set; }

    public Driver Driver { get; set; }

    public Mission(Point destination, Point source)
    {
        Id = Guid.NewGuid().ToString();

        Destination = destination;

        Source = source;

        MissionStatus = MissionStatus.Pending;
    }

    public void AssignToDriver(Driver driver)
    {
        DriverId = driver.Id;

        MissionStatus = MissionStatus.InProgress;
    }
}
