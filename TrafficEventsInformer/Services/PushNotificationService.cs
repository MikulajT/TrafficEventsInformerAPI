using FirebaseAdmin.Messaging;
using TrafficEventsInformer.Models.Fcm;

namespace TrafficEventsInformer.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public async Task SendEventStartNotificationAsync(DateTime eventStart, string[] routeNames)
        {
            bool multipleRoutes = routeNames.Length > 1;
            string formattedRouteNames = string.Join(", ", routeNames);
            SendPushNotificationAsync(new PushNotificationDto()
            {
                Title = $"Nová dopravní událost!",
                Body = $"{eventStart} začala nová dopravní událost na {(multipleRoutes ? "trasách" : "trase")} {formattedRouteNames}",
                DeviceToken = "dJlp6DutRH2dsDqXZWrvhA:APA91bEl3HxtAhrOE9bCpqCTMMUW78Mr4yLZVmE7ilWm8B6dBJsY6MywTzF5HsaEH-EwnHR6KDwreZ1AcVxc0yfAaR0f_J_vwwdHoDPOXZkP0ehzHOa3ThoD09QcEpAy2U3rfxzrhhgS"
            });
        }

        public async Task SendEventEndNotificationAsync(DateTime eventEnd, string[] routeNames)
        {
            bool multipleRoutes = routeNames.Length > 1;
            string formattedRouteNames = string.Join(", ", routeNames);
            SendPushNotificationAsync(new PushNotificationDto()
            {
                Title = $"Konec dopravní události!",
                Body = $"{eventEnd} skončila dopravní událost na {(multipleRoutes ? "trasách" : "trase")} {formattedRouteNames}",
                DeviceToken = "dJlp6DutRH2dsDqXZWrvhA:APA91bEl3HxtAhrOE9bCpqCTMMUW78Mr4yLZVmE7ilWm8B6dBJsY6MywTzF5HsaEH-EwnHR6KDwreZ1AcVxc0yfAaR0f_J_vwwdHoDPOXZkP0ehzHOa3ThoD09QcEpAy2U3rfxzrhhgS"
            });
        }

        private async Task<bool> SendPushNotificationAsync(PushNotificationDto pushNotificationDto)
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