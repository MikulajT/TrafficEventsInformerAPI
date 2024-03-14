using FirebaseAdmin.Messaging;
using TrafficEventsInformer.Models.Fcm;

namespace TrafficEventsInformer.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public async Task<bool> SendPushNotificationAsync(PushNotificationDto pushNotificationDto)
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = pushNotificationDto.Title,
                    Body = pushNotificationDto.Body
                },
                Token = pushNotificationDto.DeviceToken
            };
            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            return !string.IsNullOrEmpty(result);
        }
    }
}