namespace TrafficEventsInformer.Ef.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public ICollection<Device> Devices { get; set; }
    }
}