using TrafficEventsInformer.Ef;
using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public class TrafficRouteRepository : ITrafficRouteRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TrafficRouteRepository(ApplicationDbContext dbContext)
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

        public IEnumerable<RouteEvent> GetRouteEventNames(int routeId)
        {
            return _dbContext.RouteEvent.Where(x => x.RouteId == routeId).Select(x => new RouteEvent()
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public RouteEvent GetRouteEventDetail(int routeId, int eventId)
        {
            return _dbContext.RouteEvent.Where(x => x.RouteId == routeId && x.Id == eventId).Select(x => new RouteEvent()
            {
                TrafficRoute = new TrafficRoute()
                {
                    Id = x.RouteId,
                    Name = x.TrafficRoute.Name,
                },
                Id = x.Id,
                Type = x.Type,
                Description = x.Description,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartPointX = x.StartPointX,
                StartPointY = x.StartPointY,
                EndPointX = x.EndPointX,
                EndPointY = x.EndPointY
            }).SingleOrDefault();
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
    }
}
