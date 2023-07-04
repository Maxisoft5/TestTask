using TrueCodeTask.DataAccess.Models;

namespace TrueCodeTask.Interfaces
{
    public interface IUserService
    {
        public Task<User> AddUser(User user);
    }
}
