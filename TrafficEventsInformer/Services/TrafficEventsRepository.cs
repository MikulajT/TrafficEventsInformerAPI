using TrafficEventsInformer.Ef;
using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public class TrafficEventsRepository : ITrafficEventsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TrafficEventsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Dictionary<string, string> GetRouteEventNames(int routeId)
        {
            return _dbContext.TrafficRouteRouteEvents
                .Where(x => x.TrafficRouteId == routeId && !x.RouteEvent.Expired)
                .ToDictionary(x => x.RouteEventId, x => x.Name);
        }

        public RouteEvent GetRouteEventDetail(int routeId, string eventId)
        {
            IQueryable<RouteEvent> routeEventQuery = _dbContext.RouteEvents.Where(x => x.Id == eventId);
            TrafficRoute trafficRoute = _dbContext.TrafficRoutes.SingleOrDefault(x => x.Id == routeId);
            return routeEventQuery.Select(x => new RouteEvent()
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

        public void AddRouteEvent(RouteEvent routeEvent, TrafficRouteRouteEvent trafficRouteRouteEvent)
        {
            _dbContext.TrafficRouteRouteEvents.Add(trafficRouteRouteEvent);
            _dbContext.RouteEvents.Add(routeEvent);
            _dbContext.SaveChanges();
        }

        public bool RouteEventExists( string eventId)
        {
            return _dbContext.RouteEvents.Any(x => x.Id == eventId);
        }

        public IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents()
        {
            var expiredEvents = _dbContext.RouteEvents.Where(x => !x.Expired && DateTime.Now > x.EndDate);
            foreach (var expiredEvent in expiredEvents)
            {
                expiredEvent.Expired = true;
            }
            _dbContext.SaveChanges();
            return expiredEvents.Select(x => new ExpiredRouteEventDto()
            {
                RouteNames = x.TrafficRoutes.Select(y => y.Name).ToArray(),
                StartDate = x.StartDate,
                EndDate = x.EndDate
            });
        }

        public IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents(int routeId)
        {
            var expiredEvents = _dbContext.RouteEvents.Where(x => !x.Expired &&
                DateTime.Now > x.EndDate &&
                routeId == x.TrafficRoutes.Single(x => x.Id == routeId).Id);
            foreach (var expiredEvent in expiredEvents)
            {
                expiredEvent.Expired = true;
            }
            _dbContext.SaveChanges();
            return expiredEvents.Select(x => new ExpiredRouteEventDto()
            {
                Id = x.Id,
                RouteNames = x.TrafficRoutes.Select(y => y.Name).ToArray(),
                StartDate = x.StartDate,
                EndDate = x.EndDate
            });
        }

        public void RenameRouteEvent(int routeId, string eventId, string name)
        {
            TrafficRouteRouteEvent routeEvent = _dbContext.TrafficRouteRouteEvents.Single(x => x.TrafficRouteId == routeId && x.RouteEventId == eventId);
            routeEvent.Name = name;
            _dbContext.SaveChanges();
        }
    }
}
