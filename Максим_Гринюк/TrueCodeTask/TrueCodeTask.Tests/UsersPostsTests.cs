using Microsoft.Extensions.DependencyInjection;
using TrueCodeTask.DataAccess.Models;
using TrueCodeTask.Interfaces;
using TrueCodeTask_Tests.Abstract;

namespace TrueCodeTask_Tests
{
    public class UsersPostsTests : BaseIntegrationTest
    {
        [Fact]
        public async Task GetPostsByUserTest()
        {
            var webFactory = GetTestApplication();

            using (var scope = webFactory.Services.CreateAsyncScope())
            {
                // Arrange
                User userForTest = new()
                {
                    Name = "TestUser1",
                };
                var scopedServices = scope.ServiceProvider;
                var userService = scopedServices.GetRequiredService<IUserService>();
                var postService = scopedServices.GetRequiredService<IPostsServices>();
                var post1 = new Post()
                {
                    PublishDate = DateTime.UtcNow,
                    Content = "Test Content"
                };
                var post2 = new Post()
                {
                    PublishDate = DateTime.UtcNow,
                    Content = "Test Content2"
                };
                var savedPosts = new List<Post>();
                int postsToSearch = 2;

                // Act
                userService.ShouldNotBeNull();
                postService.ShouldNotBeNull();

                var addedUser = await userService.AddUser(userForTest);

                post1.UserId = userForTest.Id;
                var addedPost1 = await postService.AddNew(post1);
                post2.UserId = userForTest.Id;
                var addedPost2 = await postService.AddNew(post2);

                savedPosts.Add(addedPost1);
                savedPosts.Add(addedPost2);

                var usersPost = await postService.GetAllPostsByUser(userForTest.Id, postsToSearch);

                // Assert
                addedUser.Id.ShouldNotBe(0);
                addedUser.Name.ShouldBe(userForTest.Name);

                addedPost1.Id.ShouldNotBe(0);
                addedPost1.Content.ShouldBe(post1.Content);
                addedPost1.PublishDate.ShouldBe(post1.PublishDate);
                addedPost1.UserId.ShouldBe(userForTest.Id);
                addedPost2.Id.ShouldNotBe(0);
                addedPost2.Content.ShouldBe(post2.Content);
                addedPost2.PublishDate.ShouldBe(post2.PublishDate);
                addedPost2.UserId.ShouldBe(userForTest.Id);

                usersPost.Count().ShouldBe(postsToSearch);
                usersPost.Select(x => x.PublishDate).ShouldBeInOrder();

                foreach (var post in usersPost)
                {
                    var savedPost = savedPosts.FirstOrDefault(x => x.Id == post.Id).ShouldNotBeNull();
                    savedPost.Content.ShouldBe(post.Content);
                    savedPost.PublishDate.ShouldBe(post.PublishDate);
                    savedPost.UserId.ShouldBe(post.UserId);
                }

            }

            DeleteDatabase(webFactory);
        }

        [Fact]
        public async Task GetPostsByManyUsers()
        {
            // Arrange
            var webFactory = GetTestApplication();

            List<User> usersForTests = new() {
                new() { Name = "TestUser2" },
                new() { Name = "TestUser3" },
                new() { Name = "TestUser4" },
                new() { Name = "TestUser5" },
            };
            List<Post> addedPosts = new();

            using (var scope = webFactory.Services.CreateAsyncScope())
            {
                var scopedServices = scope.ServiceProvider;
                var userService = scopedServices.GetRequiredService<IUserService>();
                var postService = scopedServices.GetRequiredService<IPostsServices>();

                // Act Assert
                userService.ShouldNotBeNull();
                postService.ShouldNotBeNull();

                for (int i = 0; i < usersForTests.Count; i++)
                {
                    User? user = usersForTests[i];
                    var savedUser = await userService.AddUser(user);
                    Post post = new()
                    {
                        Content = $"TestContent by {savedUser.Name}",
                        PublishDate = DateTime.UtcNow,
                        UserId = savedUser.Id
                    };
                    var savedPost = await postService.AddNew(post);
                    addedPosts.Add(savedPost);

                }

                var posts = await postService.GetAllPostsByUsers(usersForTests.Select(x => x.Id), addedPosts.Count);
                posts.Count().ShouldBe(usersForTests.Count);
                posts.Select(x => x.PublishDate).ShouldBeInOrder();

                foreach (var post in posts)
                {
                    var user = usersForTests.FirstOrDefault(x => x.Id == post.UserId).ShouldNotBeNull();
                    post.Content.ShouldBe($"TestContent by {user.Name}");
                }

            }
            DeleteDatabase(webFactory);
        }

    }
}
