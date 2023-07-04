using TrueCodeTask.DataAccess.Models;

namespace TrueCodeTask.Interfaces
{
    public interface IPostsServices
    {
        public Task<Post> AddNew(Post post);
        public Task<IEnumerable<Post>> GetAllPostsByUser(int userId, int postsCount);
        public Task<IEnumerable<Post>> GetAllPostsByUsers(IEnumerable<int> users, int postsCount);
    }
}
