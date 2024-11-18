using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public interface IUsersRepository
    {
        void AddFcmDeviceToken(string userId, string token);
        bool UserHasToken(string userId, string token);
        IEnumerable<string> GetFcmDeviceTokens(string userId);
        IEnumerable<User> GetUsers();
        bool UserExists(string userId);
        void AddUser(string userId, string email);
    }
}