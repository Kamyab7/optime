using Application.Common.Interfaces;
using Bogus;
using NetTopologySuite.Geometries;

namespace Infrastructure.Services;

public class FakeBus : IBus
{
    public Point Publish(string driverId)
    {
        var pointFaker = new Faker<Point>()
                                          .CustomInstantiator(f =>
                                          {
                                              // Generate latitude and longitude within valid ranges
                                              double latitude = f.Random.Double(-90, 90);
                                              double longitude = f.Random.Double(-180, 180);

                                              // Create valid geometry points with SRID 4326
                                              var point = new Point(longitude, latitude) { SRID = 4326 };

                                              return point;
                                          });

        return pointFaker.Generate();
    }
}
