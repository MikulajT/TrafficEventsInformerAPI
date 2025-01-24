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
        public IEnumerable<RouteEventDto> GetRouteEvents(int routeId)
        {
            return _dbContext.TrafficRouteRouteEvents
                .Where(x => x.TrafficRouteId == routeId)
                .OrderByDescending(x => x.RouteEvent.StartDate)
                .Select(x => new RouteEventDto()
                {
                    Id = x.RouteEventId,
                    Name = x.Name,
                    StartDate = x.RouteEvent.StartDate,
                    EndDate = x.RouteEvent.EndDate
                }).ToList();
        }

        public RouteEventDetailEntities GetRouteEventDetail(int routeId, string eventId)
        {
            return new RouteEventDetailEntities()
            {
                RouteEvent = _dbContext.RouteEvents.SingleOrDefault(x => x.Id == eventId),
                TrafficRouteRouteEvent = _dbContext.TrafficRouteRouteEvents.SingleOrDefault(x => x.TrafficRouteId == routeId
                    && x.RouteEventId == eventId)
            };
        }

        public void AddRouteEvent(RouteEvent routeEvent)
        {
            _dbContext.RouteEvents.Add(routeEvent);
            _dbContext.SaveChanges();
        }

        public bool RouteEventExists(string eventId)
        {
            return _dbContext.RouteEvents.Any(x => x.Id == eventId);
        }

        //public IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents()
        //{
        //    var expiredEvents = _dbContext.RouteEvents.Where(x => !x.Expired && DateTime.Now > x.EndDate);
        //    foreach (var expiredEvent in expiredEvents)
        //    {
        //        expiredEvent.Expired = true;
        //    }
        //    _dbContext.SaveChanges();
        //    return expiredEvents.Select(x => new ExpiredRouteEventDto()
        //    {
        //        RouteNames = x.TrafficRouteRouteEvents.Select(y => y.Name).ToArray(),
        //        StartDate = x.StartDate,
        //        EndDate = x.EndDate
        //    });
        //}

        //public IEnumerable<ExpiredRouteEventDto> InvalidateExpiredRouteEvents(int routeId)
        //{
        //    var expiredEvents = _dbContext.RouteEvents.Where(x => !x.Expired &&
        //        DateTime.Now > x.EndDate &&
        //        routeId == x.TrafficRouteRouteEvents.Single(x => x.TrafficRouteId == routeId).TrafficRouteId);

        //    foreach (var expiredEvent in expiredEvents)
        //    {
        //        expiredEvent.Expired = true;
        //    }

        //    //TODO: Rewrite into single query
        //    string userId = _dbContext.TrafficRoutes.Single(x => x.Id == routeId).UserId;

        //    _dbContext.SaveChanges();

        //    return expiredEvents.Select(x => new ExpiredRouteEventDto()
        //    {
        //        Id = x.Id,
        //        RouteNames = x.TrafficRouteRouteEvents.Select(y => y.Name).ToArray(),
        //        StartDate = x.StartDate,
        //        EndDate = x.EndDate,
        //        UserId = userId
        //    });
        //}

        public void RenameRouteEvent(int routeId, string eventId, string name)
        {
            TrafficRouteRouteEvent routeEvent = _dbContext.TrafficRouteRouteEvents.Single(x => x.TrafficRouteId == routeId
                && x.RouteEventId == eventId);
            routeEvent.Name = name;
            _dbContext.SaveChanges();
        }

        public bool IsRouteEventAssignedToUser(string eventId, string userId)
        {
            return _dbContext.TrafficRouteRouteEvents.Any(x => x.RouteEventId == eventId && x.UserId == userId);
        }

        public RouteEvent GetRouteEvent(string eventId)
        {
            return _dbContext.RouteEvents.Single(x => x.Id == eventId);
        }

        public void AssignRouteEventToUser(TrafficRouteRouteEvent trafficRouteRouteEvent)
        {
            _dbContext.TrafficRouteRouteEvents.Add(trafficRouteRouteEvent);
            _dbContext.SaveChanges();
        }
    }
}
