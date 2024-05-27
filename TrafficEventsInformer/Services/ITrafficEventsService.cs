using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsService
    {
        IEnumerable<GetRouteEventNamesResponse> GetRouteEventNames(int routeId);
        GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId);
        Task SyncAllRouteEvents();
        Task<IEnumerable<GetRouteEventNamesResponse>> SyncRouteEvents(int routeId);
        void RenameRouteEvent(int routeId, string eventId, string name);

        // TODO: Remove
        Task<List<SituationRecord>> GetActiveTrafficEvents();
    }
}
