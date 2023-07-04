using Microsoft.EntityFrameworkCore;
using TrueCodeTask.DataAccess.Models;

namespace TrueCodeTask.DataAccess.EFCore
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DataContext()
        {
        }
        public DataContext(DbContextOptions options) : base(options)
        {

        }

    }
}
