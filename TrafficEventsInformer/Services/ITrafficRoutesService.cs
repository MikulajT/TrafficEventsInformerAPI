using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRoutesService
    {
        IEnumerable<GetTrafficRouteNamesResponse> GetTrafficRouteNames(string userId);
        int AddRoute(AddRouteRequestDto routeRequest);
        void DeleteRoute(int routeId);
        void RenameRoute(UpdateRouteRequest requestData);
    }
}