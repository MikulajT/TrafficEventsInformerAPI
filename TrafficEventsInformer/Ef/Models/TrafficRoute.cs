namespace TrafficEventsInformer.Ef.Models
{
    public class TrafficRoute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
        public ICollection<TrafficRouteRouteEvent> TrafficRouteRouteEvents { get; set; }
    }
}
