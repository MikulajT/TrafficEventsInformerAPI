using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRouteRepository
    {
        IEnumerable<TrafficRoute> GetTrafficRouteNames();
        IEnumerable<RouteEvent> GetRouteEventNames(int routeId);
        RouteEvent GetRouteEventDetail(int routeId, string eventId);
        void AddRoute(string routeName, string routeCoordinates);
    }
}
