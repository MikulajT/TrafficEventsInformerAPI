using TrafficEventsInformer.Models.Fcm;

namespace TrafficEventsInformer.Services
{
    public interface IPushNotificationService
    {
        Task<bool> SendPushNotificationAsync(PushNotificationDto pushNotificationDto);
    }
}