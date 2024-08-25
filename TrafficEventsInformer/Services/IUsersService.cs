using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface IUsersService
    {
        ServiceResult AddFcmDeviceToken(string userId, string token);
        bool FcmDeviceTokenExists(string userId, string token);
        IEnumerable<string> GetUserIds();
    }
}