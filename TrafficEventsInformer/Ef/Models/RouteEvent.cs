﻿namespace TrafficEventsInformer.Ef.Models
{
    public class RouteEvent
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double StartPointX { get; set; }
        public double StartPointY { get; set; }
        public double EndPointX { get; set; }
        public double EndPointY { get; set; }
        public bool Expired { get; set; }
        public ICollection<TrafficRouteRouteEvent> TrafficRouteRouteEvents { get; set; }
    }
}
