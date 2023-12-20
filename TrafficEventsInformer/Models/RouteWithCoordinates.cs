using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Models
{
    public class RouteWithCoordinates
    {
        public int RouteId { get; set; }
        public List<Trkpt> RouteCoordinates { get; set; }

        public RouteWithCoordinates(int routeId, List<Trkpt> routeCoordinates)
        {
            RouteId = routeId;
            RouteCoordinates = routeCoordinates;
        }
    }
}
