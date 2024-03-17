namespace TrafficEventsInformer.Models.Fcm
{
    public class PushNotificationDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string DeviceToken { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
