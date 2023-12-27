using TrafficEventsInformer.Ef;
using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public class TrafficEventsRepository : ITrafficEventsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TrafficEventsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<RouteEvent> GetRouteEventNames(int routeId)
        {
            return _dbContext.RouteEvents
                .Where(x => routeId == x.TrafficRoutes.Single(x => x.Id == routeId).Id && !x.Expired)
                .Select(x => new RouteEvent()
                {
                    Id = x.Id,
                    Name = x.Name
                });
        }

        public RouteEvent GetRouteEventDetail(int routeId, string eventId)
        {
            TrafficRoute trafficRoute = _dbContext.TrafficRoutes.Single(x => x.Id == routeId);
            return _dbContext.RouteEvents.Where(x => routeId == trafficRoute.Id && x.Id == eventId).Select(x => new RouteEvent()
            {
                TrafficRoutes = new List<TrafficRoute>()
                {
                    trafficRoute
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

        public void AddRouteEvent(RouteEvent routeEvent)
        {
            _dbContext.RouteEvents.Add(routeEvent);
            _dbContext.SaveChanges();
        }

        public bool RouteEventExists( string eventId)
        {
            return _dbContext.RouteEvents.Any(x => x.Id == eventId);
        }

        public void InvalidateExpiredRouteEvents()
        {
            var expiredEvents = _dbContext.RouteEvents.Where(x => !x.Expired && DateTime.Now > x.EndDate);
            foreach (var expiredEvent in expiredEvents)
            {
                expiredEvent.Expired = true;
            }
            _dbContext.SaveChanges();
        }

        public void InvalidateExpiredRouteEvents(int routeId)
        {
            var expiredEvents = _dbContext.RouteEvents.Where(x => !x.Expired &&
                DateTime.Now > x.EndDate &&
                routeId == x.TrafficRoutes.Single(x => x.Id == routeId).Id);
            foreach (var expiredEvent in expiredEvents)
            {
                expiredEvent.Expired = true;
            }
            _dbContext.SaveChanges();
        }
    }
}
