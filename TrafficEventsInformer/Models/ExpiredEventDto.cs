namespace TrafficEventsInformer.Models
{
    /// <summary>
    /// Contains expired event data
    /// </summary>
    public class ExpiredEventDto
    {
        public string EventId { get; set; }
        public DateTime EndDate { get; set; }
        public List<RouteExpiredEventDto> Routes { get; set; }
    }
}