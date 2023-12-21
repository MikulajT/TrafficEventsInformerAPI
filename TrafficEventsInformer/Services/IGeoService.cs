using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface IGeoService
    {
        Dictionary<string, WgsPoint> ConvertCoordinates(IEnumerable<SituationRecord> situations);

        WgsPoint ConvertSjtskToWgs84(double xSjtsk, double ySjtsk);

        bool AreCoordinatesWithinRadius(double lat1, double lon1, double lat2, double lon2, double radiusMeters);

        double CalculateDistanceBetweenCoordinates(double lat1, double lon1, double lat2, double lon2);

        double ToRadians(double degrees);
    }
}
