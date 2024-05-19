using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRoutesService
    {
        IEnumerable<GetTrafficRouteNamesResponse> GetTrafficRouteNames();
        int AddRoute(AddRouteRequest routeRequest);
        void DeleteRoute(int routeId);
        void UpdateRoute(UpdateRouteRequest requestData);
    }
}
