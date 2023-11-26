using TrafficEventsInformer.Models.UsersRoute;

namespace TrafficEventsInformer.Services
{
    public interface IGeoService
    {
        IEnumerable<Trkpt> GetUsersRoute();
        IEnumerable<SituationRecord> GetEventsOnUsersRoute(IEnumerable<Trkpt> routeCoordinates, IEnumerable<SituationRecord> situations);
    }
}
