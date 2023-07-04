using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeTask.DataAccess.EFCore;

namespace TrueCodeTask_Tests.Abstract
{
    public abstract class BaseIntegrationTest
    {
        protected WebApplicationFactory<Program> GetTestApplication()
        {
            var webFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);
                    var uuId = Guid.NewGuid().ToString();
                    var dbContextString = $"Server=.;Database=testTrueCodeDb-{uuId};Trusted_Connection=True;Integrated security=true;TrustServerCertificate=true";
                    services.AddDbContext<DataContext>(o =>
                    {
                        o.UseSqlServer(dbContextString);
                    });
                    var provider = services.BuildServiceProvider();
                    var context = provider.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                });
            });
            return webFactory;
        }

        protected void DeleteDatabase(WebApplicationFactory<Program> factory)
        {
            using (var scope = factory.Services.CreateAsyncScope())
            {
                var scopedServices = scope.ServiceProvider;
                var cxt = scopedServices.GetRequiredService<DataContext>();
                cxt.Database.EnsureDeleted();

            }
        }
    }
}
