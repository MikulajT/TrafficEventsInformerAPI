using TrafficEventsInformer.Models;

namespace TrafficEventsInformer.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public ServiceResult AddFcmDeviceToken(string userId, string token)
        {
            if (_usersRepository.FcmDeviceTokenExists(userId, token))
            {
                return ServiceResult.ResourceExists;
            }
            else
            {
                _usersRepository.AddFcmDeviceToken(userId, token);
                return ServiceResult.Success;
            }
        }

        public bool FcmDeviceTokenExists(string userId, string token)
        {
            return _usersRepository.FcmDeviceTokenExists(userId, token);
        }

        public IEnumerable<string> GetUserIds()
        {
            return _usersRepository.GetUserIds();
        }
    }
}