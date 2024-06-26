﻿namespace TrafficEventsInformer.Models
{
    public class RouteEventDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public int DaysRemaining { get; set; }
    }
}