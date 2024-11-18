using TrafficEventsInformer.Ef.Models;
using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public interface IUsersService
    {
        ServiceResult AddFcmDeviceToken(string userId, string token);
        bool UserHasToken(string userId, string token);
        IEnumerable<User> GetUsers();
        ServiceResult AddUser(AddUserRequestDto requestDto);
    }
}