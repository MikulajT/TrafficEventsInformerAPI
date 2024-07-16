using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRoutesService
    {
        IEnumerable<GetTrafficRouteNamesResponse> GetTrafficRouteNames(string userId);
        int AddRoute(AddRouteRequest routeRequest, string userId);
        void DeleteRoute(int routeId);
        void UpdateRoute(UpdateRouteRequest requestData);
    }
}