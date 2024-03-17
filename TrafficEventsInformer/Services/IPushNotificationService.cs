namespace TrafficEventsInformer.Services
{
    public interface IPushNotificationService
    {
        Task SendEventStartNotificationAsync(DateTime eventStart, string[] routeNames, string eventId);
        Task SendEventEndNotificationAsync(DateTime eventEnd, string[] routeNames, string eventId);
    }
}