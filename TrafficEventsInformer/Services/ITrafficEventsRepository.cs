using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsRepository
    {
        Dictionary<string, string> GetRouteEventNames(int routeId);
        RouteEventDetailEntities GetRouteEventDetail(int routeId, string eventId);
        void AddRouteEvent(RouteEvent routeEvent, TrafficRouteRouteEvent trafficRouteRouteEvent);
        bool RouteEventExists(string eventId);
        IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents();
        IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents(int routeId);
        void RenameRouteEvent(int routeId, string eventId, string name);
    }
}
