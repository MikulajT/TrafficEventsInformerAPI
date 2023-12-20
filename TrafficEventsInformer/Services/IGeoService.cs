using TrafficEventsInformer.Models;
using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Services
{
    public interface IGeoService
    {
        IEnumerable<RouteWithCoordinates> GetUsersRouteWithCoordinates();
        IEnumerable<SituationRecord> GetEventsOnUsersRoute(IEnumerable<Trkpt> routeCoordinates, IEnumerable<SituationRecord> situations);
    }
}
