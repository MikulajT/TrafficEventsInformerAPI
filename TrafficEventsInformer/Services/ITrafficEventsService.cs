using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsService
    {
        IEnumerable<RouteEventDto> GetRouteEvents(int routeId);
        GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId);
        Task SyncAllRouteEvents();
        Task<GetRouteEventsResponse> SyncRouteEvents(int routeId);
        void RenameRouteEvent(int routeId, string eventId, string name);

        // TODO: Remove
        Task<List<SituationRecord>> GetActiveTrafficEvents();
    }
}
