using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRouteService
    {
        IEnumerable<GetTrafficRouteNamesResponse> GetTrafficRouteNames();
        IEnumerable<GetRouteEventNamesResponse> GetRouteEventNames(int routeId);
        GetRouteEventDetailResponse GetRouteEventDetail(int routeId, string eventId);
        Task AddRouteAsync(AddRouteRequest routeRequest);
        Task SyncUsersRouteEvents();
    }
}
