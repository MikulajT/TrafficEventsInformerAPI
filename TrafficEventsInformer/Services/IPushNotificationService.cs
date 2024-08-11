namespace TrafficEventsInformer.Services
{
    public interface IPushNotificationService
    {
        Task SendEventStartNotificationAsync(DateTime eventStart, string[] routeNames, int routeId, string eventId, string userId);
        Task SendEventEndNotificationAsync(DateTime eventEnd, string[] routeNames, int routeId, string eventId, string userId);
    }
}