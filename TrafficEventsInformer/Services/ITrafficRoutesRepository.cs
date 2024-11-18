using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRoutesRepository
    {
        IEnumerable<TrafficRoute> GetTrafficRouteNames(string userId);
        int AddRoute(string routeName, string routeCoordinates, string userId);
        IEnumerable<TrafficRoute> GetRoutes(string userId);
        TrafficRoute GetRoute(int routeId);
        void DeleteRoute(int routeId);
        void RenameRoute(UpdateRouteRequest requestData);
    }
}
