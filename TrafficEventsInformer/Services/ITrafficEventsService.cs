using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsService
    {
        IEnumerable<RouteEventDto> GetRouteEvents(int routeId);
        GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId);
        Task SyncRouteEventsAsync();
        Task SyncRouteEventsAsync(string userId);
        void RenameRouteEvent(int routeId, string eventId, string name);
    }
}