namespace TrafficEventsInformer.Models
{
    public class GetRouteEventDetailResponse
    {
        public int EventId { get; set; }
        public int EventType { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRemaining { get; set; }
    }
}
