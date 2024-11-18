using TrafficEventsInformer.Ef.Models;
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
            if (_usersRepository.UserHasToken(userId, token))
            {
                return ServiceResult.ResourceExists;
            }
            else
            {
                _usersRepository.AddFcmDeviceToken(userId, token);
                return ServiceResult.Success;
            }
        }

        public bool UserHasToken(string userId, string token)
        {
            return _usersRepository.UserHasToken(userId, token);
        }

        public IEnumerable<User> GetUsers()
        {
            return _usersRepository.GetUsers();
        }

        public ServiceResult AddUser(AddUserRequestDto requestDto)
        {
            if (_usersRepository.UserExists(requestDto.Id))
            {
                return ServiceResult.ResourceExists;
            }
            else
            {
                _usersRepository.AddUser(requestDto.Id, requestDto.Email);
                return ServiceResult.Success;
            }
        }
    }
}