using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Models
{
    public class RouteEventDetailEntities
    {
        public RouteEvent RouteEvent { get; set; }
        public TrafficRoute TrafficRoute { get; set; }
        public TrafficRouteRouteEvent TrafficRouteRouteEvent { get; set; }
    }
}
