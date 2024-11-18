using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsRepository
    {
        IEnumerable<RouteEventDto> GetRouteEvents(int routeId);
        RouteEventDetailEntities GetRouteEventDetail(int routeId, string eventId);
        void AddRouteEvent(RouteEvent routeEvent);
        bool RouteEventExists(string eventId);
        IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents();
        IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents(int routeId);
        void RenameRouteEvent(int routeId, string eventId, string name);
        bool IsRouteEventAssignedToUser(string eventId, string userId);
        RouteEvent GetRouteEvent(string eventId);
        void AssignRouteEventToUser(TrafficRouteRouteEvent trafficRouteRouteEvent);
    }
}