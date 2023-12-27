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
            return _dbContext.RouteEvent
                .Where(x => x.RouteId == routeId && !x.Expired)
                .Select(x => new RouteEvent()
                {
                    Id = x.Id,
                    Name = x.Name
                });
        }

        public RouteEvent GetRouteEventDetail(int routeId, string eventId)
        {
            return _dbContext.RouteEvent.Where(x => x.RouteId == routeId && x.Id == eventId).Select(x => new RouteEvent()
            {
                TrafficRoute = new List<TrafficRoute>()
                {
                    new TrafficRoute()
                    {
                        Id = x.RouteId,
                        Name = x.TrafficRoute.Single(x => x.Id == routeId).Name,
                    }
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
            _dbContext.RouteEvent.Add(routeEvent);
            _dbContext.SaveChanges();
        }

        public bool RouteEventExists(int routeId, string eventId)
        {
            return _dbContext.RouteEvent.Any(x => x.RouteId == routeId && x.Id == eventId);
        }

        public void InvalidateExpiredRouteEvents()
        {
            var expiredEvents = _dbContext.RouteEvent.Where(x => !x.Expired && DateTime.Now > x.EndDate);
            foreach (var expiredEvent in expiredEvents)
            {
                expiredEvent.Expired = true;
            }
            _dbContext.SaveChanges();
        }
    }
}
