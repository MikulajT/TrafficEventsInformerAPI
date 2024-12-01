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

        public void AddFcmDeviceToken(string userId, string token)
        {
            if (!_usersRepository.UserHasToken(userId, token))
            {
                _usersRepository.AddFcmDeviceToken(userId, token);
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

        public void AddUser(AddUserRequestDto requestDto)
        {
            if (!_usersRepository.UserExists(requestDto.Id))
            {
                _usersRepository.AddUser(requestDto.Id, requestDto.Email);
            }
        }
    }
}