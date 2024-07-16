namespace TrafficEventsInformer.Ef.Models
{
    public class TrafficRouteRouteEvent
    {
        public int TrafficRouteId { get; set; }
        public TrafficRoute TrafficRoute { get; set; }
        public string RouteEventId { get; set; }
        public RouteEvent RouteEvent { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}