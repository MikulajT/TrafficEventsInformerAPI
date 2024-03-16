namespace TrafficEventsInformer.Services
{
    public interface IPushNotificationService
    {
        Task SendEventStartNotificationAsync(DateTime eventStart, string[] routeNames);
        Task SendEventEndNotificationAsync(DateTime eventEnd, string[] routeNames);
    }
}