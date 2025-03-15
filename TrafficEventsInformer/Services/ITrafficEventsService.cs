using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsService
    {
        IEnumerable<RouteEventDto> GetRouteEvents(int routeId);
        GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId);
        void RenameRouteEvent(int routeId, string eventId, string name);
        Task InvalidateExpiredRouteEventsAsync();
        Task SyncRouteEventsAsync(string userId, int routeId);
        Task SyncEvents(Stream trafficEvents);
    }
}