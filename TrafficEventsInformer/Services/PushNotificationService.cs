using FirebaseAdmin.Messaging;
using TrafficEventsInformer.Models.Fcm;

namespace TrafficEventsInformer.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IUsersRepository _usersRepository;

        public PushNotificationService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task SendEventStartNotificationAsync(DateTime eventStart, string[] routeNames, int routeId, string eventId, string userId)
        {
            bool multipleRoutes = routeNames.Length > 1;
            string formattedRouteNames = string.Join(", ", routeNames);
            string[] fcmDeviceTokes = _usersRepository.GetFcmDeviceTokens(userId).ToArray();

            foreach (string fcmDeviceToken in fcmDeviceTokes)
            {
                _ = SendPushNotificationAsync(new PushNotificationDto()
                {
                    Title = $"Nová dopravní událost!",
                    Body = $"{eventStart} začala nová dopravní událost na {(multipleRoutes ? "trasách" : "trase")} {formattedRouteNames}",
                    DeviceToken = fcmDeviceToken,//"dJlp6DutRH2dsDqXZWrvhA:APA91bEl3HxtAhrOE9bCpqCTMMUW78Mr4yLZVmE7ilWm8B6dBJsY6MywTzF5HsaEH-EwnHR6KDwreZ1AcVxc0yfAaR0f_J_vwwdHoDPOXZkP0ehzHOa3ThoD09QcEpAy2U3rfxzrhhgS",
                    Data = new Dictionary<string, string>()
                    {
                        { "routeId", routeId.ToString() },
                        { "eventId", eventId }
                    }
                });
            }
        }

        public async Task SendEventEndNotificationAsync(DateTime eventEnd, string[] routeNames, int routeId, string eventId, string userId)
        {
            bool multipleRoutes = routeNames.Length > 1;
            string formattedRouteNames = string.Join(", ", routeNames);
            string[] fcmDeviceTokes = _usersRepository.GetFcmDeviceTokens(userId).ToArray();

            foreach (string fcmDeviceToken in fcmDeviceTokes)
            {
                _ = SendPushNotificationAsync(new PushNotificationDto()
                {
                    Title = $"Konec dopravní události!",
                    Body = $"{eventEnd} skončila dopravní událost na {(multipleRoutes ? "trasách" : "trase")} {formattedRouteNames}",
                    DeviceToken = fcmDeviceToken,
                    Data = new Dictionary<string, string>()
                    {
                        { "routeId", routeId.ToString() },
                        { "eventId", eventId }
                    }
                });
            }
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
                Token = pushNotificationDto.DeviceToken,
                Data = pushNotificationDto.Data
            };
            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            return !string.IsNullOrEmpty(result);
        }
    }
}