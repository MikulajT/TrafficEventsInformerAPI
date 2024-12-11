using TrafficEventsInformer.Ef;
using TrafficEventsInformer.Ef.Models;

namespace TrafficEventsInformer.Services
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UsersRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddFcmDeviceToken(string userId, string token)
        {
            Device device = new Device()
            {
                FcmToken = token,
                UserId = userId
            };
            _dbContext.Devices.Add(device);

            //User user = _dbContext.Users.Single(x => x.Id == userId);

            //if (user.Devices == null)
            //{
            //    user.Devices = new List<Device>();
            //}

            //if (!user.Devices.Any(x => x.FcmToken == token))
            //{
            //    user.Devices.Add(device);
            //}

            _dbContext.SaveChanges();
        }

        public bool UserHasToken(string userId, string token)
        {
            return _dbContext.Users.Any(x => x.Id == userId && x.Devices.Select(x => x.FcmToken).Contains(token));
        }

        public IEnumerable<string> GetFcmDeviceTokens(string userId)
        {
            return _dbContext.Users
                .Where(x => x.Id == userId)
                .SelectMany(x => x.Devices)
                .Select(x => x.FcmToken);
        }

        public IEnumerable<User> GetUsers()
        {
            return _dbContext.Users.ToList();
        }

        public bool UserExists(string userId)
        {
            return _dbContext.Users.Any(x => x.Id == userId);
        }

        public void AddUser(string userId, string email)
        {
            _dbContext.Users.Add(new User()
            {
                Id = userId,
                Email = email
            });
            _dbContext.SaveChanges();
        }
    }
}