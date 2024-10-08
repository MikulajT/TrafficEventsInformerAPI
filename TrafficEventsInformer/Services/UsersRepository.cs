﻿using TrafficEventsInformer.Ef;
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

            _dbContext.Users.Add(new User()
            {
                Id = userId,
                FcmDeviceToken = token
            });
            _dbContext.SaveChanges();
        }

        public bool FcmDeviceTokenExists(string userId, string token)
        {
            return _dbContext.Users.Any(x => x.Id == userId && x.FcmDeviceToken == token);
        }

        public IEnumerable<string> GetFcmDeviceTokens(string userId)
        {
            return _dbContext.Users.Where(x => x.Id == userId).Select(x => x.FcmDeviceToken);
        }

        public IEnumerable<string> GetUserIds()
        {
            return _dbContext.Users.Select(x => x.Id);
        }
    }
}