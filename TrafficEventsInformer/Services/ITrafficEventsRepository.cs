using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsRepository
    {
        IEnumerable<RouteEvent> GetRouteEventNames(int routeId);
        RouteEvent GetRouteEventDetail(int routeId, string eventId);
        void AddRouteEvent(RouteEvent routeEvent);
        bool RouteEventExists(string eventId);
        IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents();
        IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents(int routeId);
    }
}
