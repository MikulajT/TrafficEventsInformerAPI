using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRoutesService
    {
        IEnumerable<GetTrafficRouteNamesResponse> GetTrafficRouteNames();
        Task AddRouteAsync(AddRouteRequest routeRequest);
        void DeleteRoute(int routeId);
    }
}
