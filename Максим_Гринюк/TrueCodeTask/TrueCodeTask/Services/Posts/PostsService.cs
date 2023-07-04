using Microsoft.EntityFrameworkCore;
using TrueCodeTask.DataAccess.EFCore;
using TrueCodeTask.DataAccess.Models;
using TrueCodeTask.Interfaces;

namespace TrueCodeTask.Services.Posts
{
    public class PostsService : IPostsServices
    {
        private readonly DataContext _dataContext;
        public PostsService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Post> AddNew(Post post)
        {
            post.User = null;
            await _dataContext.Posts.AddAsync(post);
            await _dataContext.SaveChangesAsync();
            return post;
        }

        public async Task<IEnumerable<Post>> GetAllPostsByUser(int userId, int postsCount)
        {
            return await _dataContext.Posts.Include(x => x.User)
                .Where(x => x.UserId == userId).OrderBy(x => x.PublishDate)
                .Take(postsCount).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllPostsByUsers(IEnumerable<int> usersIds, int postsCount)
        {
            return await _dataContext.Posts.Include(x => x.User)
                .Where(x => usersIds.Contains(x.UserId)).OrderBy(x => x.PublishDate)
                .Take(postsCount).ToListAsync();
        }
    }
}
