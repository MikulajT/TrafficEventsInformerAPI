using FirebaseAdmin.Messaging;
using TrafficEventsInformer.Models.Fcm;

namespace TrafficEventsInformer.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public async Task SendEventStartNotificationAsync(DateTime eventStart, string[] routeNames, string eventId)
        {
            bool multipleRoutes = routeNames.Length > 1;
            string formattedRouteNames = string.Join(", ", routeNames);
            SendPushNotificationAsync(new PushNotificationDto()
            {
                Title = $"Nová dopravní událost!",
                Body = $"{eventStart} začala nová dopravní událost na {(multipleRoutes ? "trasách" : "trase")} {formattedRouteNames}",
                DeviceToken = "cZx4iNgXRmWaQa8A1ZCMO3:APA91bHaO04snK7ou516Tcsh2M-a_OO_1wsK7QEc3sx5o0SNEo1SPKshDbagBSiLvF1jlZImF8VvOF-gDMF2uoDLEzSOwhuJ7fjWmMSmr0IY6_xjwNRkjH627FDlNbVvcHo3j-79XZEm",//"dJlp6DutRH2dsDqXZWrvhA:APA91bEl3HxtAhrOE9bCpqCTMMUW78Mr4yLZVmE7ilWm8B6dBJsY6MywTzF5HsaEH-EwnHR6KDwreZ1AcVxc0yfAaR0f_J_vwwdHoDPOXZkP0ehzHOa3ThoD09QcEpAy2U3rfxzrhhgS",
                Data = new Dictionary<string, string>()
                {
                    {"eventId", eventId}
                }
            });
        }

        public async Task SendEventEndNotificationAsync(DateTime eventEnd, string[] routeNames, string eventId)
        {
            bool multipleRoutes = routeNames.Length > 1;
            string formattedRouteNames = string.Join(", ", routeNames);
            SendPushNotificationAsync(new PushNotificationDto()
            {
                Title = $"Konec dopravní události!",
                Body = $"{eventEnd} skončila dopravní událost na {(multipleRoutes ? "trasách" : "trase")} {formattedRouteNames}",
                DeviceToken = "dJlp6DutRH2dsDqXZWrvhA:APA91bEl3HxtAhrOE9bCpqCTMMUW78Mr4yLZVmE7ilWm8B6dBJsY6MywTzF5HsaEH-EwnHR6KDwreZ1AcVxc0yfAaR0f_J_vwwdHoDPOXZkP0ehzHOa3ThoD09QcEpAy2U3rfxzrhhgS",
                Data = new Dictionary<string, string>()
                {
                    {"eventId", eventId}
                }
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
                Token = pushNotificationDto.DeviceToken,
                Data = pushNotificationDto.Data
            };
            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            return !string.IsNullOrEmpty(result);
        }
    }
}