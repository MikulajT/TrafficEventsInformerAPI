using TrafficEventsInformer.Ef;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public class TrafficRoutesRepository : ITrafficRoutesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TrafficRoutesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<TrafficRoute> GetTrafficRouteNames(string userId)
        {
            return _dbContext.TrafficRoutes
                .Where(x => x.UserId == userId)
                .Select(x => new TrafficRoute()
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public int AddRoute(string routeName, string routeCoordinates, string userId)
        {
            var route = new TrafficRoute()
            {
                Name = routeName,
                Coordinates = routeCoordinates,
                UserId = userId
            };
            _dbContext.TrafficRoutes.Add(route);
            _dbContext.SaveChanges();
            return route.Id;
        }

        public IEnumerable<TrafficRoute> GetRoutes(string userId)
        {
            return _dbContext.TrafficRoutes.Where(x => x.UserId == userId);
        }

        public TrafficRoute GetRoute(int routeId)
        {
            return _dbContext.TrafficRoutes.Single(x => x.Id == routeId);
        }

        public void DeleteRoute(int routeId)
        {
            // Get joining table records
            var trafficRouteRouteEvents = _dbContext.TrafficRouteRouteEvents.Where(x => x.TrafficRouteId == routeId).ToList();

            // Get Route events records
            var routeEventIDs = trafficRouteRouteEvents.Select(x => x.RouteEventId);
            var routeEvents = _dbContext.RouteEvents.Where(x => routeEventIDs.Contains(x.Id));

            // Get route record
            var route = _dbContext.TrafficRoutes.Single(x => x.Id == routeId);

            _dbContext.TrafficRouteRouteEvents.RemoveRange(trafficRouteRouteEvents);
            _dbContext.RouteEvents.RemoveRange(routeEvents);
            _dbContext.TrafficRoutes.Remove(route);

            _dbContext.SaveChanges();
        }

        public void UpdateRoute(UpdateRouteRequest requestData)
        {
            var route = _dbContext.TrafficRoutes.Single(x => x.Id == requestData.RouteId);
            route.Name = requestData.RouteName;
            _dbContext.SaveChanges();
        }
    }
}
