using System.ComponentModel.DataAnnotations;

namespace TrafficEventsInformer.Ef.Models
{
    public class TrafficRoute
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }
        public List<RouteEvent> Events { get; set; }
    }
}
