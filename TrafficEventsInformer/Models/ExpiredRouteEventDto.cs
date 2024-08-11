namespace TrafficEventsInformer.Models
{
    public class ExpiredRouteEventDto
    {
        public string Id { get; set; }
        public string[] RouteNames { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserId { get; set; }
    }
}