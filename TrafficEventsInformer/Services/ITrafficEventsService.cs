using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficEventsService
    {
        IEnumerable<GetRouteEventNamesResponse> GetRouteEventNames(int routeId);
        GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId);
        Task SyncAllRouteEvents();
        Task<IEnumerable<GetRouteEventNamesResponse>> SyncRouteEvents(int routeId);
    }
}
