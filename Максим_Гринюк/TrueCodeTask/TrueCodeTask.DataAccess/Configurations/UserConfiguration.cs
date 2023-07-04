using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrueCodeTask.DataAccess.Models;

namespace TrueCodeTask.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(obj => obj.Id);
            builder.HasIndex(obj => obj.Name).IsUnique();
            builder.HasMany(x => x.Posts).WithOne(x => x.User);
        }
    }
}
