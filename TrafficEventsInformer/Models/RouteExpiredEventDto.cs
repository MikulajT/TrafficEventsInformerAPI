namespace TrafficEventsInformer.Models
{
    /// <summary>
    /// Contains route data for expired event
    /// </summary>
    public class RouteExpiredEventDto
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public string UserId { get; set; }
    }
}
