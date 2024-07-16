using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsService
    {
        IEnumerable<RouteEventDto> GetRouteEvents(int routeId, string userId);
        GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId, string userId);
        Task SyncAllRouteEvents(string userId);
        Task<IEnumerable<RouteEventDto>> SyncRouteEventsAsync(int routeId, string userId);
        void RenameRouteEvent(int routeId, string eventId, string name, string userId);
    }
}