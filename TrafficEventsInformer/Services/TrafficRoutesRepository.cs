using TrafficEventsInformer.Ef;
using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public class TrafficRoutesRepository : ITrafficRoutesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TrafficRoutesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<TrafficRoute> GetTrafficRouteNames()
        {
            return _dbContext.TrafficRoute.Select(x => new TrafficRoute()
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public void AddRoute(string routeName, string routeCoordinates)
        {
            _dbContext.TrafficRoute.Add(new TrafficRoute()
            {
                Name = routeName,
                Coordinates = routeCoordinates
            });
            _dbContext.SaveChanges();
        }

        public IEnumerable<TrafficRoute> GetUsersRoutes()
        {
            return _dbContext.TrafficRoute;
        }

        public void DeleteRoute(int routeId)
        {
            var route = _dbContext.TrafficRoute.Single(x => x.Id == routeId);
            _dbContext.TrafficRoute.Remove(route);
            _dbContext.SaveChanges();
        }
    }
}
