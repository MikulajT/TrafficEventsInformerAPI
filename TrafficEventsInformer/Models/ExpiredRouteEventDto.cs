namespace TrafficEventsInformer.Models
{
    public class ExpiredRouteEventDto
    {
        public string[] RouteNames { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}