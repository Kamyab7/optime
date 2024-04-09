using NetTopologySuite.Geometries;

namespace Application.Common.Interfaces;

public interface IBus
{
    Point Publish(string driverId);
}
