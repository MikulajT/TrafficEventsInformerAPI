namespace TrafficEventsInformer.Services
{
    public interface IUsersRepository
    {
        void AddFcmDeviceToken(string userId, string token);
        bool FcmDeviceTokenExists(string userId, string token);
        IEnumerable<string> GetFcmDeviceTokens(string userId);
        IEnumerable<string> GetUserIds();
    }
}