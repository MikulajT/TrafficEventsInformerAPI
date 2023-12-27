using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsRepository
    {
        IEnumerable<RouteEvent> GetRouteEventNames(int routeId);
        RouteEvent GetRouteEventDetail(int routeId, string eventId);
        void AddRouteEvent(RouteEvent routeEvent);
        bool RouteEventExists(string eventId);
        void InvalidateExpiredRouteEvents();
        void InvalidateExpiredRouteEvents(int routeId);
    }
}
