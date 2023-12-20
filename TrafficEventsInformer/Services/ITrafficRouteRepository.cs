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
        bool RouteEventExists(int routeId, string eventId);
        void AddRouteEvent(RouteEvent routeEvent);
        void DeleteRoute(int routeId);
        void InvalidateExpiredRouteEvents();
    }
}
