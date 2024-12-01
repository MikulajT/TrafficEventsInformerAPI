using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Models
{
    public class RouteCoordinates
    {
        public int RouteId { get; set; }
        public List<Trkpt> Coordinates { get; set; }

        public RouteCoordinates(int routeId, List<Trkpt> routeCoordinates)
        {
            RouteId = routeId;
            Coordinates = routeCoordinates;
        }
    }
}
