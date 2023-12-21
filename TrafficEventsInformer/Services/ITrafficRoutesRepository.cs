using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public interface ITrafficRoutesRepository
    {
        IEnumerable<TrafficRoute> GetTrafficRouteNames();
        void AddRoute(string routeName, string routeCoordinates);
        IEnumerable<TrafficRoute> GetUsersRoutes();
        void DeleteRoute(int routeId);
    }
}
