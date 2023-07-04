using Microsoft.EntityFrameworkCore;
using TrueCodeTask.DataAccess.EFCore;
using TrueCodeTask.DataAccess.Models;
using TrueCodeTask.Interfaces;

namespace TrueCodeTask.Services.Users
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        public UserService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<User> AddUser(User user)
        {
            user.Posts = null;
            var s = _dataContext.Database.GetConnectionString();
            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
            return user;
        }
    }
}
