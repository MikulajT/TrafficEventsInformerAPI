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
            return _dbContext.TrafficRoutes.Select(x => new TrafficRoute()
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public void AddRoute(string routeName, string routeCoordinates)
        {
            _dbContext.TrafficRoutes.Add(new TrafficRoute()
            {
                Name = routeName,
                Coordinates = routeCoordinates
            });
            _dbContext.SaveChanges();
        }

        public IEnumerable<TrafficRoute> GetUsersRoutes()
        {
            return _dbContext.TrafficRoutes;
        }

        public TrafficRoute GetUsersRoute(int routeId)
        {
            return _dbContext.TrafficRoutes.Single(x => x.Id == routeId);
        }

        public void DeleteRoute(int routeId)
        {
            var route = _dbContext.TrafficRoutes.Single(x => x.Id == routeId);
            _dbContext.TrafficRoutes.Remove(route);
            _dbContext.SaveChanges();
        }
    }
}
