using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface IUsersService
    {
        void AddFcmDeviceToken(string userId, string token);
        bool UserHasToken(string userId, string token);
        IEnumerable<User> GetUsers();
        void AddUser(AddUserRequestDto requestDto);
    }
}