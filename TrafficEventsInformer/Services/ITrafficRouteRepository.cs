using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRouteRepository
    {
        IEnumerable<TrafficRoute> GetTrafficRouteNames();
        IEnumerable<RouteEvent> GetRouteEventNames(int routeId);
        RouteEvent GetRouteEventDetail(int routeId, string eventId);
        void AddRoute(string routeName, string routeCoordinates);
        IEnumerable<TrafficRoute> GetUsersRoutes();
        public bool RouteEventExists(int routeId, string eventId);
        public void AddRouteEvent(RouteEvent routeEvent);
    }
}
